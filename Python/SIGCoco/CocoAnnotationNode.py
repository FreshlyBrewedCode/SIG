import json
from SIGCoco.coco_utils import create_sub_mask_annotation, create_sub_masks
from SIGPython import SIGNode, SIGValueType, nodeinput, nodeoutput
from SIGPython.utils import measuretime
from PIL import Image
import io
import json


class CocoAnnotationNode(SIGNode):
    def __init__(self):
        self._mask = None
        self._id = 0
        self._is_crowd = False
        self._category_id = 0
        self._image_id = 0
        self._keypoints = None
        self.annotation_json = ""

    @nodeinput("Id", SIGValueType.INT)
    def id(self, value):
        self._id = value

    @nodeinput("Image Id", SIGValueType.INT)
    def image_id(self, value):
        self._image_id = value

    @nodeinput("Keypoints", SIGValueType.JSON)
    def keypoints(self, value):
        self._keypoints = json.loads(value)

    @nodeinput("Is Crowd", SIGValueType.BOOL)
    def is_crowd(self, value):
        self._is_crowd = value

    @nodeinput("Category Id", SIGValueType.INT)
    def category_id(self, value):
        self._category_id = value

    @nodeinput("Mask", SIGValueType.TEXTURE)
    def mask_texture(self, raw_texture):
        self._mask = self._bytes_to_image(raw_texture)

    @nodeoutput("JSON", SIGValueType.STRING)
    def json(self):
        return self.annotation_json

    @measuretime(3)
    def process(self):
        sub_masks = self._get_sub_masks()
        self._get_mask_annotations(sub_masks)

    @measuretime(3)
    def _get_sub_masks(self):
        return create_sub_masks(self._mask)

    @measuretime(3)
    def _get_mask_annotations(self, sub_masks):

        items = sub_masks.items()
        if len(items) != 1:
            raise Exception("Mask can only contain one sub mask.")

        (color, sub_mask) = items[0]

        self.annotation_json = json.dumps(
            create_sub_mask_annotation(
                sub_mask,
                self._image_id,
                self._category_id,
                self._id,
                self._is_crowd,
                self._keypoints,
            )
        )

    @measuretime(3)
    def _bytes_to_image(self, raw_bytes):
        return Image.open(io.BytesIO(bytearray(raw_bytes)))
