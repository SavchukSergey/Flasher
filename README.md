# Flasher

MicroFlasher is a programmer tool for flashing Atmel and Microchip microcontrollers
Use on your own risk

# Protocols

Flasher connects to your device via COM port either real port or virtual port.
Currently there are two protocols implemented stk500 v1 and bit bang.

## Stk500 version 1
Flasher has stk500v1 protocol implemented.
Stk500 protocol implies that you have either intermediate controller or you have a bootloader on target controller. The board must be connected via COM port (physical or virtual).
This protocol was used for programming stk500 boards (1st version) and also is used by arduino boards.
For more information about protocol see [AVR061](http://www.atmel.com/Images/doc2525.pdf)

### Bootloader
To program device with bootloader, i.e. arduino board, make sure you toggled "Use Reset" checkbox settings in order to make your device's bootloader run and intercept incoming requests.

### Intermediate controller
If you use intermediate controller to program other ("target") device. Please uncheck "Use Reset" option.

### Microchip devices
There is stk500 protocol implementation for PIC microcontrollers (see related projects).
Make sure you have selected proper devices in settings menu. This will switch intermediate controller into PIC programming state.

## Com Bit Bang
Flasher can interact with devices by using so called bit banging. This implies using of special wires of COM-port that are connected directly to your device, thus eliminating intermediate controller and saving your budget. But please note that this may not work correctly for virtual com ports.
