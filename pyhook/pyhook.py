import socket
import stackless
import code
import sys

# socket wrapper
class probe_sw:
    def __init__(self, s):
        self.s = s
    def read(self, len):
        return self.s.recv(len)
    def write(self, str):
        return self.s.send(str)
    def readline(self):
        return self.read(256) # lazy implementation for quick testing

# listening socket
probe_sock = socket.socket()
probe_sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
probe_sock.bind(('127.0.0.1', 2112))
probe_sock.listen(10)

# listening tasklet
def probe_accept(s):
    while True:
        c, a = s.accept()
        c = probe_sw(c)
        
        sys.stdin = c
        sys.stdout = c
        sys.stderr = c
        
        # should break if connection is dropped
        try:
            code.interact()
        except:
            pass
        
        # I wanted to kill the socket on clean exit()
        # but it doesn't seem to work?
        try:
            c.s.shutdown(SHUT_RDWR)
            c.s.close()
        except:
            pass
        
        # restore original stds
        sys.stdin = sys.__stdin__
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
        
        stackless.schedule()

# k, go!
stackless.tasklet(probe_accept)(probe_sock)
