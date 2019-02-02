# Tello SKD Emulator
Tired of powering the Tello on and off, burning battery and dealing with overheading problems. Time to write an emulator.

# Based on the Ryze Robotics Tello SDK V2.0
https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

# Limitations
Real Tello target IP is fixed at 192.168.10.1 and requires connecting to the Tello network with SSID "TELLO-[tello_id]".
Emulated Tello listens on local network, so localhost or device's IP will be the target.

# Ports
Ports are identical to real Tello.
- command listener receives on 8889
- video reports on 11111
- state reports on 8890
