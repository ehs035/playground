using System.Security.Claims;

namespace BrokerPortal.Api.Security;

public static class ClaimsExtensions
{
	/// <summary>
	/// Build a strongly-typed claims context (string IDs) from the principal.
	/// - Trims values
	/// - Case-insensitive claim type matching
	/// - FullName: DisplayName -> Given + Family
	/// - IsAdmin: explicit flag OR any role contains "admin"
	/// </summary>
	public static ClaimsContext GetClaimsContext(this ClaimsPrincipal user, ClaimNameOptions? names = null)
	{
		names ??= new ClaimNameOptions();

		var brokerId = NormalizeId(user.FindFirstValueCI(names.BrokerId));
		var agencyId = NormalizeId(user.FindFirstValueCI(names.AgencyId));
		var role = NormalizeId(user.FindFirstValueCI(names.Role));

		var fullName = FirstNonEmpty(
			user.FindFirstValueCI(names.DisplayName)?.Trim(),
			ComposeName(
				user.FindFirstValueCI(names.FirstName)?.Trim(),
				user.FindFirstValueCI(names.LastName)?.Trim()
			)
		);

		//var isAdmin =
		//	TryGetBool(user, names.IsAdminFlag) is true ||
		//	RolesContainAdmin(user, names.RoleClaims);

		return new ClaimsContext(
			BrokerId: brokerId,
			AgencyId: agencyId,
			FullName: fullName,
			Role: role

		);
	}

	/// <summary>Case-insensitive FindFirstValue.</summary>
	public static string? FindFirstValueCI(this ClaimsPrincipal user, string claimType)
	{
		var claim = user.Claims.FirstOrDefault(
			c => string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase));
		return claim?.Value;
	}

	/// <summary>Normalize an ID: trims and returns null if empty/whitespace.</summary>
	private static string? NormalizeId(string? s)
	{
		var t = s?.Trim();
		return string.IsNullOrWhiteSpace(t) ? null : t;
	}

	/// <summary>Return the first non-empty string.</summary>
	private static string? FirstNonEmpty(params string?[] values)
	{
		foreach (var v in values)
		{
			if (!string.IsNullOrWhiteSpace(v)) return v;
		}
		return null;
	}

	/// <summary>Compose "Given Family" when available.</summary>
	private static string? ComposeName(string? given, string? family)
	{
		var g = string.IsNullOrWhiteSpace(given) ? null : given;
		var f = string.IsNullOrWhiteSpace(family) ? null : family;
		if (g is null && f is null) return null;
		if (g is null) return f;
		if (f is null) return g;
		return $"{g} {f}";
	}

	/// <summary>Parse boolean flags robustly: true/false, 1/0, yes/no.</summary>
	private static bool? TryGetBool(ClaimsPrincipal user, string claimType)
	{
		var raw = user.FindFirstValueCI(claimType);
		if (string.IsNullOrWhiteSpace(raw)) return null;

		var s = raw.Trim();
		if (bool.TryParse(s, out var b)) return b;
		if (s is "1" or "0") return s == "1";
		if (s.Equals("yes", StringComparison.OrdinalIgnoreCase)) return true;
		if (s.Equals("no", StringComparison.OrdinalIgnoreCase)) return false;
		return null;
	}

	/// <summary>Check roles claims for any "admin" occurrence (case-insensitive).</summary>
	private static bool RolesContainAdmin(ClaimsPrincipal user, IEnumerable<string> roleClaimTypes)
	{
		foreach (var type in roleClaimTypes)
		{
			foreach (var c in user.FindAll(type))
			{
				var v = c.Value?.Trim();
				if (!string.IsNullOrEmpty(v) &&
					v.Contains("admin", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}
}