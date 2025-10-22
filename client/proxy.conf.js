const PROXY_CONFIG = [
  {
    context: [
      "/api/",
      "/hub/"
    ],
    target: "https://localhost:5001",
    secure: false,
    changeOrigin: true,
    ws: true,
    logLevel: "debug"
  }
];

module.exports = PROXY_CONFIG;
