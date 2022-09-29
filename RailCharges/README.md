# Rail Charges

Provides multiple configurable improvements for Railgunner sniping experience:

- Rail charges display in scope mode
- Custom crosshair in non-scope mode
- Configurable FOV in scope mode
- Scope Wind-up time configuration
- (Experimental) Angular aim compensation when entering scope mode

Contact me using handle amadare#8308 in Discord.

## Features

### Rail charges display

When Railgunner have one or more Backup Magazines, shot count will be displayed in scope. When shot is on cooldown it will be slightly transparent and will become opaque when shot is ready.

### Change crosshair in non-scope mode

By changing `Crosshair` value in configuration, default non-scope crosshair can be changed.

You can also disable decorative outer rectangle (Yellow) with `Remove Outer Rectangle` option and disable reload boost indicator (square with lightning after successful reload) with `Disable Boosted Indicator` option. So it is possible to remove all aiming HUD if you want.

![crosshair variants](.\Readme\crosshairs.jpg)


### FOV and Wind-up time confiration

You can change in-scope FOV by using `Scope FOV override` option. 30 is default in-game value. Lower value - greater zoom. If you set it to 60, it will be the same as in non-scope mode.

You can change animation duration for entering scope mode by using `WindUp duration override` option. Value is in seconds. 0.1 is default in-game value. If set to 0, transition will be instant.

### (Experimental) Angular aim compensation

Because when Railgunner scope, camera will only translate to character position, but not rotate to character view, aiming point will be slightly lower than before entering scope (see picture below). This will be noticeable more for short range. This mod can compensate for that by finding what point is in screen center before entering the scope and adds extra camera rotation during entering scope animation, making you aim same point. 

![angular](.\Readme\angular_compensation.jpg)


## Version History

### 0.2.0

- [FEATURE] Scope FOV override
- [FEATURE] Change crosshair in non-scope mode
- [FEATURE] Angular compensation

### 0.1.1

- fixed bug when custom component would be duplicated on each scene change

### 0.1.0

- now working in multiplayer
- improved codebase

### 0.0.1 - Initial release