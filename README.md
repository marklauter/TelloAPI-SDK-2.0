# Tello API for SDK 2.0 in C#
The project includes a flight controller, Tello emulator, UDP messenger, script builder, and two console samples.

Project in action: https://www.youtube.com/watch?v=l6AOf1QZb9g

## Introduction
I started by playing with a fork of the TelloLib (aTello) project but abandoned it when Ryze upgraded their text based SDK. With the latest firmware updates and SDK 2.0, the drone communication is relatively reliable. However, the Tello sometimes responds to the SDK initialization command with gobbledygook instead of the expected 'ok'. When this happens, additional commands may be executed by the Tello, but won't always ACK. Rebooting the Tello and reconnecting to its network usually resolves the issue. I have yet to try attaching the Tello to my local network. Maybe that will solve the issue once and for all.

## Project Goals
My goals for this project are to refine my dependency injection skills, experiment with machine learning and video processing, play with a thing that I can fly from my desk, and begin to iterate on the larger problem of programmable drone behavior.

## Flight Controller's Async Command Queue - Why is it coded like this?
Tello doesn't respond to commands as I expected after first skim of the SDK 1.0 documentation. I thought it would ACK each command as it was received, but this isn't the case. There doesn't appear to be an internal command queue, so Tello doesn't ACK a command until it has been executed. For example, if you send a command like UP (10), it might take a few seconds to complete the maneuver and ACK, but if you send something like FORWARD (500, 10) it's going to take a lot longer to complete the maneuver and you won't get an ACK until it's complete. This makes timing out unreceived commands a bit more complex. Ideally the drone would ACK the command, execute the maneuver, and ACK completion.

Since the Tello operates synchronously, the flight controller had to provide an asynchronous interface to the Tello messaging system. We can't have a UI sitting around waiting for a 20 second command to complete, right? So command messages are queued up as they are received, program control is returned to the main thread, and a queue consumer running in a seperate thread pulls an item off the queue when it detects that Tello is ready for the next message.

## Work Plan
### February - March 2019 - COMPLETE
* messaging interfaces
* UDP messenger
* Tello SDK 2.0 emulator
* sample projects with dependency injection
* automated tests
* simplified scipting

### April 2019 Goals
* UWP UI - command buttons tab, script tab, real time operations log display, gauges based on aviation 6 pack
* operations and flight data logging in SQLite
* script improvements
* video (decoding H264) without crap UWP media control
* incorporate OpenCV

### May 2019 Goals
* Nuget
* Add physics to emulator

## Projects within Tello.API.sln

### SDK 2.0
I have a first generation Tello. I've read in the forums that an EDU version is required to use the mission pad and some of the new commands. Since I can't test the Tello with the mission pad I don't know how to simulate it's behavior it in the emulator. If someone has an EDU and wants to uncomment the EDU commands in the Commands enum and implement them, I'd be happy to accept a solid pull request. If someone wants to donate an EDU model, I'd be happy to finish the mission pad commands myself.

### Tello.Controller
This is the core of the system. See the static Program class constructors in Tello.Emulator.SDKV2.Demo or Tello.Udp.Demo to see how to instantiate FlightController. Using it is a matter of wiring up the events and then calling command methods. The first command must be EnterSdkMode.

### Tello.Messaging
Provides injectable messenger services for Tello. These interfaces are implemented by two projects: Tello.Udp, which communicates with Tello, and Tello.Emulator.SDKV2, which roughly emulates Tello behavior. The FlightController in Tello.Controller is coded to the Tello.Messaging interfaces and is implementation independent.

### Tello.Udp
Implements Tello.Messaging via UDP access to a live Tello drone. A connection to the Tello's WIFI is required.

### Tello.Emulator.SDKV2
Implements Tello.Messaging by emulating Tello drone behavior. No network connection is required.

### Tello.Scripting
For now a script is a simple JSON list of commands and parameters. The commands are based on the Tello.Messaging.Commands enum. There's a ScriptBuilder to generate command scripts and a TelloScript class that can be used by the FlightController. The ScriptBuilder outputs JSON that can be parsed by the TelloScript. It isn't rocket science. Once you see how the scripts are generated, you can probably edit the JSON directly more easily than using the ScriptBuilder.
I'll add more complex script support once I have the UWP demo working. 

### Tests
Writing tests is boring. When I feel really professional and high-speed, I'll add to them, but I'm not in a hurry. If someone wants to round them out, they would have my eternal gratitude and a gift certificate to Taco Bell.
