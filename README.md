# CAST Summer Project - Greenfly Population Simulation

# Installation (Windows x64 only)
 - Head to 'Releases' and download the latest release as a .zip file.
 - Extract and run the executable.
 - This is self-contained and should not require the dotnet framework to be installed on your machine.

# Usage
 - Press the topmost button on the menu to change values, and press `tab` to cycle through to the next text field. 
 - Survival rates should be floating-point values between zero and one, but are not strictly constrained, so be nice. 
 - The number of generations must be between 5 and 25, or the application will throw an exception. 
 - To run the simulation and get the results at-a-glance, press "Start Simulation". 
 - To export these values, press the "Export CSV..." button and input a filename ending in .csv, then press `enter`. The resulting csv file will be in the export/ directory.

# Known issues/limitations
 - The custom UI framework being used is still pretty rough:
     - it is possible to pick up several windows at once
     - text in text fields cannot be selected
     - buttons will still appear to be hovered/clicked, even when obstructed and not actually clickable.
