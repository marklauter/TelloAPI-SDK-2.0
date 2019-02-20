# Tello API for SDK 2.0 in C#
The project includes a flight controller, Tello emulator, UDP messenger, script builder, and two console samples.

I started by playing with a fork of the TelloLib (aTello) project but abandoned it when Ryze upgraded their SDK. With the latest firmware updates and SDK 2.0, the drone communication is relatively reliable. However, the Tello sometimes responds to the SDK initialization command with gobbledygook instead of the expected 'ok'. When this happens, additional commands may be executed by the Tello, but won't always ACK. Rebooting the Tello and reconnecting to its network usually resolves the issue. I have yet to try attaching the Tello to my local network. Maybe that will solve the issue once and for all.

My goals for this project are to refine my dependency injection skills, experiment with machine learning and video processing, play with a thing that I can fly from my desk, and begin to iterate on the larger problem of programmable drone behavior.

## SDK 2.0
I have a first generation Tello. I've read in the forums that an EDU version is required to use the mission pad and some of the new commands. Since I can't test the Tello with the mission pad I don't know how to simulate it's behavior it in the emulator. If someone has an EDU and wants to uncomment the EDU commands in the Commands enum and implement them, I'd be happy to accept a solid pull request. If someone wants to donate an EDU model, I'd be happy to finish the mission pad commands.

## Tello.Controller
This is the core of the system. See the static Program class constructors in Tello.Emulator.SDKV2.Demo or Tello.Udp.Demo to see how to instantiate FlightController. Using it is a matter of wiring up the events and then calling command methods. The first command must be EnterSdkMode.

## Tello.Messaging
Provides injectable messenger services for Tello. These are implemented by Tello.Udp and Tello.Emulator.SDKV2. The FlightController is coded to Tello.Messaging and is implmentation independent.

## Tello.Udp
Implements Tello.Messaging via UDP access to a live Tello drone. A connection to the Tello's WIFI is required.

## Tello.Emulator.SDKV2
Implements Tello.Messaging by emulating Tello drone behavior. No network connection is required.

## Tello.Scripting
Provides a ScriptBuilder to generate command scripts and a TelloScript class that can be used by the FlightController. The ScriptBuilder outputs JSON that can be parsed by the TelloScript. It isn't rocket science. Once you see how the scripts are generated, you can probably edit the JSON directly more easily than using the ScriptBuilder.

## Tests
Writing tests is boring. When I feel really professional and high speed I'll add to them, but I'm not in a hurry. If someone wants to round them out they would have my eternal gratitude and a gift certificate to Taco Bell.
