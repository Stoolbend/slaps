![SLAPS Project Logo](/frontend/src/assets/slaps_logo.png)

# ShinyLAN Advanced Projector System (SLAPS)

SLAPS is an evolution of big-screen information display & community interaction for events. The project takes inspiration from existing products and builds upon the same concept with redesigned features & support for content from more services.

**SLAPS is currently in development, and will be pushed here once the first workable prototype is ready.**

The project comes in 2 parts: the frontend & the server, which are both built separately and then combined before deployment.

- The frontend is written in Vue.js, and will connect to the server component using traditional methods along with ASP.NET Core SignalR.
- The server is written in ASP.NET Core 3.1, and will host a traditional REST API along with multiple SignalR endpoints for realtime communications. It will also host the compiled frontend, allowing for simple deployment by the end user.
