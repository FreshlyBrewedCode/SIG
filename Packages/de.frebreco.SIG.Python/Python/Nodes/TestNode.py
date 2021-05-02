from SIGPython import SIGNode, SIGValueType, nodeinput, nodeoutput


class MyNode(SIGNode):

    def __init__(self):
        super().__init__()
        self.v = False

    @nodeinput("In", SIGValueType.BOOL)
    def input(self, value):
        self.v = bool(value)

    @nodeoutput("Out", SIGValueType.BOOL)
    def output(self):
        return not self.v


class ExampleAddNode(SIGNode):
    def __init__(self):
        self._a = 0
        self._b = 0
        self._out = 0

    def process(self):
        self._out = self._a + self._b

    @nodeinput("A", SIGValueType.FLOAT)
    def a(self, value):
        self._a = value

    @nodeinput("B", SIGValueType.FLOAT)
    def b(self, value):
        self._b = value

    @nodeoutput("Out", SIGValueType.FLOAT)
    def output(self):
        return self._out
