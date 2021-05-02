# Though Python for Unity currently uses Python 2.7,
# We are ready for the coming of Python 3.x.
from __future__ import division, print_function
import time
import threading
import sys
import socket
import os

# Import SIG api
from SIGPython import *
from SIGPython.registry import SIGNodeRegistry


# This import fixes our sys.path if it's missing the package root.
# It also adds threading.get_ident if it's missing.
import unity_client


try_to_connect = True

# A client requires a unique name.
client_name = "de.frebreco.sigpython.clients.worker"


class SIGWorkerClientService(unity_client.UnityClientService):
    """
    A generic client that allows eval, exec, and access to the globals.

    Override the name and you can use this service to connect to any framework.
    """

    def __init__(self):
        self._globals = dict()
        self._registry = SIGNodeRegistry()

    # The client name is used for logging and calls from Unity.
    def exposed_client_name(self):
        return client_name

    def exposed_globals(self):
        return self._globals

    def exposed_init(self, nodes):
        print("Initialize Python Worker...")
        print("Loading nodes...")
        print(nodes)
        for path in nodes:
            print("\t" + str(path))
            self._registry.load_nodes(path)

    def exposed_process_node(self, node_name, inputs):
        node = self._registry.create_node(node_name)

        # print("[" + node_name + "]")
        # print("\t" + "In:  " + str(inputs))

        node.push_inputs(inputs)
        node.process()
        outputs = node.pull_outputs()

        # print("\t" + "Out: " + str(outputs))

        return outputs

    # The on_server_shutdown service tells the client that Unity has disconnected from it.
    # The following function determines how this is handled.
    # The 'invite_retry' flag is a bloolean.

    def exposed_on_server_shutdown(self, invite_retry):
        global try_to_connect
        try_to_connect = invite_retry
        super(SIGWorkerClientService,
              self).exposed_on_server_shutdown(invite_retry)


"""
This client is set up to be run from the command-line, or via C# API (PythonRunner.SpawnClient).
An exterior application could also be made to connect to Unity.
"""
if __name__ == '__main__':
    # This is the loop which maintains the connection to Unity.
    # It handles reconnection, and whether it is needed (try_to_connect may be false).
    while try_to_connect:
        time.sleep(0.5)
        try:
            # Here starts a new thread connecting to Unity.
            # It listens to incoming messages, and uses the defined service.
            c = unity_client.connect(SIGWorkerClientService)
        except socket.error:
            print("failed to connect; try again later")
            continue

        print("connected")
        try:
            c.thread.join()  # Wait for KeyboardInterrupt (^c or server quit)
        except KeyboardInterrupt:
            c.close()
            c.thread.join()  # Wait for the thread to notice the close()
        print("disconnected")
