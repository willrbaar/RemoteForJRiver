# RemoteForJRiver
RemoteForJRiver is intended to be a Remote Control For Operating JRiver Media Center from an IPad, plus provide streaming support and other controls for an entertainment center.  RemoteForJRiver is currently very early in development.

RemoteForJRiver is a UWP app and is envisioned to have these features:
1. Used in conjunction with JRIVER Media Center and using many of the features of JRiver.
2. Controls to AV equipment that either have a internet control interface or have other interfaces, such as legacy serial interface (modules such as Global Cache would be need to provide the interface).
2. Controls to lighting which are either z-wave (Global Cache needed) or wifi controled.
3. Ability to Stream Video from the internet, plus simply switching between JRiver Media Center and Internet Streaming.
4. Ability to retain the JRiver 10 Foot remote control interface, or use a user control that is more along the lines of today's computer mouse/keyboard control.

The initial prototype of RemoteForJRiver uses another app called JRiverServer.  This server provides web pages to an IPad that provides the control interface for the user.  The IPad's Safari can be used, or an app like IRule has modes that will also support this serer.  The control interface from the IPad to RemoteForJRiver is a Websocket.
