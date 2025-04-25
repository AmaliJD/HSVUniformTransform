<h1>HSVUniformTransform</h1>

I am trying to transform this gradient in HSV space so each row is of the same perceptual lightness. My hope is that the resulting gradient is a smooth rainbow minimizing any artifacts that come from the vastly different brightnesses of different hues being aligned.

Below is the original HSV gradient. It's 255x255 and ordered so each column is its own hue, and each column goes from white at the top, to the max saturation and value hue in the middle, to black on the bottom.

<h5>HSV-linear</h5>

![hsv-linear](https://github.com/user-attachments/assets/ed8a035f-c5f4-49a5-bb27-290f52c43c3e)

This is my first attempt at the transformation, made in Python. I used the luminance formula L = 0.299*R + 0.587*G + 0.114*B to get the lightness of each color in the column and organize them so every row was a color of the same lightness value.

<h5>HSV-luminance</h5>

![hsv-uniform-gamma 1](https://github.com/user-attachments/assets/91499858-4937-46c4-ad36-75869a4a066b)

Converting this to b&w with most software will result in a perfectly smooth gradient from white to black. Yet you may be struck like me that the rainbow itself certainly does not look perceptually uniform across the rows. That's because this formula alone doesn't actually reflect the correct perceptual lightness of a color. Since it's what b&w converters use, the conversion results in a uniform gradient.

<h5>HSV-luminance-grayscale</h5>

![hsv-uniform-gamma 1 -gray 1](https://github.com/user-attachments/assets/e5d97efa-fa12-4a4e-ae2b-c8aa7e44651e)

Applying a power function helps a lot with the rainbow gradient. A color science researcher acknowledged my resulting gradient looked pretty good. Comparing it to a gradient made in the OK-LAB color space, it's pretty close! The spacing of the OK-LAB hues reflects it's own perceptually uniform gradation of the hues, as opposed to HSV's non-perceptually uniform gradation, but the shape is there.

<h5>HSV-luminance-gamma[2.4] ------------- OK-LAB</h5>

![hsv-uniform-gamma 2 4](https://github.com/user-attachments/assets/210f8989-353f-4947-ab0e-4b7e8099bed4)
![oklch](https://github.com/user-attachments/assets/76aa0ee3-a085-42e2-9b8a-b609e0711a15)

But he pointed out that an issue with "perceptual lightness" is that because of the Helmholtz–Kohlrausch effect, darker colors like blue can still look brighter than lighter colors because of it's saturation. You can see in the gradient that even though blue is placed low on the spectrum, it pops out way more than the other colors of "equal" lightness. This means the brightness of a color is both influenced by it's percieved luminance AND it's saturation, which is a BITCH becuase those properties so not combine to create one unified property I'd like to call vibrance. It also means this whole project is kinda absurd, I cannot rank colors by lightness because their saturation adds a different kind of brightness to the color that's spereate from the lightness yet still creates an overall sensation of the color being more vibrant and this "brighter". But I can still try my best! I searched for better lightness functions that could take the Helmholtz–Kohlrausch effect into account, cuz I was not satisfied with these gradients.

The CAM16 model's Q value is a brightness value that takes the Helmholtz–Kohlrausch effect into account so I used that, tho fsr wound up with strange artifacts in the resulting image.

<h5>HSV-CAM16-Q-gamma[2.2]-power[0.6]</h5>
  
![hsv-cam16-Q-gamma 2 2 -Qpow 0 6](https://github.com/user-attachments/assets/4066e08f-e4ed-4eec-8158-6a1ca1ddc4e7)

Finally, I learned of a more accurate lightness function provided by CIE, and produced this.

<h5>HSV-CIE-gamma[2.2]</h5>

![hsv-uniformCIE-gamma 2 2](https://github.com/user-attachments/assets/7dad5121-9ed5-490b-b8bf-b86e641807fe)

Honestly, all my results are pretty good, I could have stopped there, but I couldn't help feel like they weren't good ENOUGH. Just looking at them I felt like certain color's positions could be improved. Thus, I created this Unity project where I, like a maniac, MANUALLY PLACE EACH HUE IN THE CORRECT SPOT! I can't do all 255x255 colors, so I'm starting smaller on 48x25 and hopefully I can upscale it later, but that's still 1200 colors right now. It's very slow and tedious work. Here's my progress so far!

<h5>HSV-manual-[48x25]</h5>
  
![image](https://github.com/user-attachments/assets/e6cb359d-6110-4c86-ad92-e8905c802c2c)
