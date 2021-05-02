from PIL import Image
from skimage import measure
import numpy as np
from shapely.geometry import Polygon, MultiPolygon
import json


def create_sub_masks(mask_image):
    width, height = mask_image.size

    # Initialize a dictionary of sub-masks indexed by RGB colors
    sub_masks = {}
    for x in range(width):
        for y in range(height):
            # Get the RGB values of the pixel
            pixel = mask_image.getpixel((x, y))[:3]

            # If the pixel is not black...
            if pixel != (0, 0, 0):
                # Check to see if we've created a sub-mask...
                pixel_str = str(pixel)
                sub_mask = sub_masks.get(pixel_str)
                if sub_mask is None:
                    # Create a sub-mask (one bit per pixel) and add to the dictionary
                    # Note: we add 1 pixel of padding in each direction
                    # because the contours module doesn't handle cases
                    # where pixels bleed to the edge of the image
                    sub_masks[pixel_str] = Image.new("1", (width + 2, height + 2))

                # Set the pixel value to 1 (default is 0), accounting for padding
                sub_masks[pixel_str].putpixel((x + 1, y + 1), 1)

    return sub_masks


def create_sub_mask_annotation(
    sub_mask, image_id, category_id, annotation_id, is_crowd, keypoints=[]
):
    # Find contours (boundary lines) around each sub-mask
    # Note: there could be multiple contours if the object
    # is partially occluded. (E.g. an elephant behind a tree)
    contours = measure.find_contours(sub_mask, 0.5, positive_orientation="low")

    segmentations = []
    polygons = []
    for contour in contours:
        # Flip from (row, col) representation to (x, y)
        # and subtract the padding pixel
        for i in range(len(contour)):
            row, col = contour[i]
            contour[i] = (col - 1, row - 1)

        # Make a polygon and simplify it
        poly = Polygon(contour)
        poly = poly.simplify(1.0, preserve_topology=True)
        polygons.append(poly)
        segmentation = np.array(poly.exterior.coords).ravel().astype(int).tolist()
        segmentations.append(segmentation)

    # Combine the polygons to calculate the bounding box and area
    multi_poly = MultiPolygon(polygons)
    x, y, max_x, max_y = multi_poly.bounds
    width = max_x - x
    height = max_y - y
    bbox = (int(x), int(y), int(width), int(height))
    area = multi_poly.area

    annotation = {
        "id": annotation_id,
        "image_id": image_id,
        "category_id": category_id,
        "segmentation": segmentations,
        "area": area,
        "bbox": bbox,
        "iscrowd": int(bool(is_crowd)),
    }

    if not keypoints is None:
        annotation["keypoints"] = keypoints
        annotation["num_keypoints"] = len(keypoints) / 3

    return annotation


# mask_img = Image.open(
#     "D:/Dokumente/Bachelorarbeit/Image Generator/Assets/mask.png")
# sub_masks = create_sub_masks(mask_img)
# annotation = ""

# for color, sub_mask in sub_masks.items():
#     #category_id = category_ids[image_id][color]
#     annotation = create_sub_mask_annotation(sub_mask, 0, 0, 0, False)
# #     annotations.append(annotation)
# #     annotation_id += 1
# # image_id += 1

# print(annotation)
