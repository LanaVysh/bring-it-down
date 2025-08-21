// playwright.config.js
// Playwright configuration for UI tests

/** @type {import('@playwright/test').PlaywrightTestConfig} */
const config = {
  use: {
    baseURL: 'https://localhost:7175/',
    ignoreHTTPSErrors: true,
    headless: true,
  },
  testDir: './',
};

module.exports = config;
