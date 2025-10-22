const PROXY_CONFIG = [
  {
    context: [
      "/api/",
      "/hub/"
    ],
    target: "https://skinet-felix.azurewebsites.net/",
    secure: false,
    changeOrigin: true,
    ws: true,
    logLevel: "debug"
  }
];

module.exports = PROXY_CONFIG;
