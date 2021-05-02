from inspect import isclass
from pkgutil import iter_modules
from pathlib import Path
from importlib import import_module
from SIGPython import SIGNode


def load_nodes(path):
    nodes = []

    # iterate through the modules in the current package
    package_dir = str(Path(path).resolve().parent)
    print("Package: " + package_dir)
    for (importer, package_name, _) in iter_modules([package_dir]):
        print("Module: " + package_name)
        full_package_name = '%s.%s' % (package_dir, package_name)
        # import the module and iterate through its attributes
        module = importer.find_module(package_name
                                      ).load_module(full_package_name)
        for attribute_name in dir(module):
            attribute = getattr(module, attribute_name)

            if isclass(attribute) and issubclass(attribute, SIGNode) and attribute is not SIGNode:
                print("\t..." + attribute_name)
                nodes.append(attribute)
                # Add the class to this package's variables
                globals()[attribute_name] = attribute
    return nodes
