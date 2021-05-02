import functools
import sys


class SIGType:
    INPUT = "input"
    OUTPUT = "output"


class SIGValueType:
    FLOAT = "single"
    INT = "int"
    BOOL = "bool"
    STRING = "string"
    TEXTURE = "texture"
    JSON = "json"


class SIGProperty:
    def __init__(self, func, name, type, value_type):
        self.func = func
        self.name = name
        self.type = type
        self.value_type = value_type

    def __str__(self):
        return self.name + " (" + self.type + ", " + self.value_type + ")"


def _sig_annotator(fn, property):
    @functools.wraps(fn)
    def wrapped_f(*args, **kwargs):
        return fn(*args, **kwargs)
    wrapped_f.sig_property = property
    return wrapped_f


def nodeinput(name, type):
    def wrapped(fn):
        return _sig_annotator(fn, SIGProperty(fn, name, SIGType.INPUT, type))
    return wrapped


def nodeoutput(name, type):
    def wrapped(fn):
        return _sig_annotator(fn, SIGProperty(fn, name, SIGType.OUTPUT, type))
    return wrapped


class SIGNode:

    def __init__(self):
        pass

    def process(self):
        pass

    @property
    def node_inputs(self):
        return SIGNode.get_inputs(self.__class__)

    @property
    def node_outputs(self):
        return SIGNode.get_outputs(self.__class__)

    def push_inputs(self, inputs):
        node_inputs = self.node_inputs
        for k in node_inputs.keys():
            # 'inputs' does not behave like a normal dictionary after it was received
            # from Unity. dict.keys, dict.get and probably other dict methods will cause
            # the worker thread to freeze. 'k in inputs' is the only safe way to check if
            # the key exists in the inputs dictionary.
            if k in inputs:
                node_inputs[k].func(self, inputs[k])

    def pull_outputs(self):
        outputs = {}
        node_outputs = self.node_outputs
        for k in node_outputs.keys():
            value = node_outputs[k].func(self)
            outputs[k] = value
        return outputs

    @staticmethod
    def get_inputs(node_class):
        inputs = {}
        for i in [p for p in SIGNode.get_sig_properties(node_class) if p.type == SIGType.INPUT]:
            inputs[i.func.__name__] = i
        return inputs

    @staticmethod
    def get_outputs(node_class):
        outputs = {}
        for o in [p for p in SIGNode.get_sig_properties(node_class) if p.type == SIGType.OUTPUT]:
            outputs[o.func.__name__] = o
        return outputs

    @staticmethod
    def get_sig_properties(node_class):
        output = []
        members = node_class.__dict__
        for k in members.keys():
            prop = getattr(members[k], "sig_property", None)
            if not prop is None:
                output.append(prop)
        return output

    @staticmethod
    def create_node_by_name(node_class_name):
        node_class = SIGNode._str_to_class(node_class_name)
        if node_class is None or not issubclass(node_class, SIGNode):
            raise NodeNotFoundException(node_class_name +
                                        " is not a valid node class name")
        return node_class()

    @staticmethod
    def _str_to_class(str):
        return getattr(sys.modules[__name__], str, None)


# Exceptions

class SIGException(Exception):
    """Base class for all exceptions raised by SIGPython."""


class NodeNotFoundException(SIGException):
    """Raised if a node of a given name could not be found."""


# -----------------------------------------------------------------
#                               TESTS
# -----------------------------------------------------------------

# CREATE BY NAME, INPUTS & OUTPUTS

# class ExampleAddNode(Node):
#     def __init__(self):
#         self._a = 0
#         self._b = 0
#         self._out = 0

#     def process(self):
#         self._out = self._a + self._b

#     @nodeinput("A", SIGValueType.FLOAT)
#     def a(self, value):
#         self._a = value

#     @nodeinput("B", SIGValueType.FLOAT)
#     def b(self, value):
#         self._b = value

#     @nodeoutput("Out", SIGValueType.STRING)
#     def output(self):
#         return self._out

# node = Node.create_node_by_name("ExampleAddNode")

# node.push_inputs({'a': 5, 'b': 4})
# node.process()
# print(node.pull_outputs())
