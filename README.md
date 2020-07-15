# DSPEmulator
Convert audio files to mp3 with channels delays and equalization.
![DSPEmulator Main Window](https://user-images.githubusercontent.com/54595612/81505493-3f506700-92f8-11ea-8a18-cf1d5647c470.png)
![DSPEmulator Effects](https://user-images.githubusercontent.com/54595612/87562945-50866d00-c6c7-11ea-8637-1f95c78ec5aa.png)

# For Whom?
For those who want to hear how the dsp processor affects the sound picture in the car.
# Signal Processing Pipeline
* Conversion from mono to stereo (if necessary)
* Performing channels delays (set in milliseconds)
* Channels equalization
* Channels volume
* Channels phase switching
* High-pass and Low-pass Filters
* Saving the result in mp3 320 kb/s
# How To Calculate Delays?
Measure the distance from your head to each twitter. 
For the nearest speaker you need to set the delay equal to the difference in distance to the speakers divided by the speed of sound (344 m/s).
For example, the distance to the left twitter is 1 meter, and to the right 1.3 meters. 
The left channel delay is (1.3m - 1m) / 344 m/s = 0.00087 seconds = 0.87 milliseconds.
