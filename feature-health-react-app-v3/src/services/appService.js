import { appData } from '../data/apps';

function delay(ms = 450) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function getApps() {
  await delay();
  return appData;
}

export async function getAppById(appId) {
  await delay();
  return appData.find((item) => item.id === appId) ?? null;
}
