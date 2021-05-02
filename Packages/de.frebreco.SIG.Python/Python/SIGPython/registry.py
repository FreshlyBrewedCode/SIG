from . import SIGNode, SIGException, NodeNotFoundException
from os import SEEK_SET, sys, path
from inspect import isclass
from pkgutil import iter_modules
from pathlib import Path


class SIGNodeRegistry:

    def __init__(self):
        self._nodes = {}

    def load_nodes(self, module_path):
        nodes = SIGNodeRegistry._load_nodes_from_module(module_path)
        existing = self._nodes.keys()

        for node in nodes:
            node_name = node.__name__
            if node_name in existing:
                raise NodeAlreadyRegisteredException(
                    "A node with name '" + node_name + "' has already been registered")
            self._nodes[node_name] = node

    def get_node_class(self, node_name):
        return self._nodes.get(node_name, None)

    def create_node(self, node_name):
        # print("Trying to get node " + node_name)
        node_class = self.get_node_class(node_name)
        # print("Node class: " + str(node_class))

        if node_class is None:
            raise NodeNotFoundException(
                node_name + " does not exist in the registry")
        return node_class()

    @staticmethod
    def _load_nodes_from_module(module_path):
        nodes = []

        # Resolve package directory
        package_dir = str(Path(module_path).resolve().parent)
        sys.path.insert(0, path.abspath(
            path.join(path.dirname(module_path), "..")))

        # Iterate through the modules in the current package
        for (importer, package_name, _) in iter_modules([package_dir]):
            full_package_name = '%s.%s' % (package_dir, package_name)

            # Import the module and iterate through its attributes
            module = importer.find_module(package_name
                                          ).load_module(package_name)
            for attribute_name in dir(module):
                attribute = getattr(module, attribute_name)

                # Check if 'attribute' is a user defined SIGNode
                if isclass(attribute) and issubclass(attribute, SIGNode) and attribute is not SIGNode:
                    nodes.append(attribute)
                    globals()[attribute_name] = attribute
        return nodes

# Exceptions


class NodeAlreadyRegisteredException(SIGException):
    """Raised if a node with the same name has already been registered."""
