# DSPEmulator
Convert audio files to mp3 with channel delays and equalization.
# Build
The project is compiled using Visual Studio Community 2019. Require [NAudio](https://github.com/naudio/NAudio), [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json).
# Usage
Just drag and drop files onto the program and enter the channel delays. Equalization parameters are stored in a file 'equalizer.json'. To create an example file of equalization parameters, run the program with the argument '--create-equalizer'. 
# For Whom?
For those who want to hear how the dsp processor affects the sound picture in the car.
# How To Calculate Delays?
Measure the distance from your head to each twitter. 
For the nearest speaker you need to set the delay equal to the difference in distance to the speakers divided by the speed of sound (344 m/s).
For example, the distance to the left twitter is 1 meter, and to the right 1.3 meters. 
The left channel delay is (1.3m - 1m) / 344 m/s = 0.00087 seconds = 0.87 milliseconds.
# Further Improvements
* GUI
* Preview with effects applied
