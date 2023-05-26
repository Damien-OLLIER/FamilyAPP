from PIL import Image
import os

input_folder = "imageFolder"
output_folder = "compressedFolder"

# Create the output folder if it doesn't exist
if not os.path.exists(output_folder):
    os.makedirs(output_folder)

# Iterate over the files in the input folder
for filename in os.listdir(input_folder):
    input_path = os.path.join(input_folder, filename)
    output_path = os.path.join(output_folder, filename)

    # Open the image
    with Image.open(input_path) as image:
        # Convert the image to RGB (JPEG does not support transparency)
        image = image.convert("RGB")

        # Compress and save the image with reduced quality (adjust the quality value as needed)
        image.save(output_path, format="JPEG", optimize=True, quality=85)
