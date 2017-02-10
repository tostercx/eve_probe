import socket
import stackless
import threading
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
probe_sock.setblocking(0)

# interactive thread
def probe_shell_thred(c):
    try:
        c = probe_sw(c)
        
        sys.stdin = c
        sys.stdout = c
        #sys.stderr = c
        
        code.interact()
        
        c.s.shutdown(SHUT_RDWR)
        c.s.close()
    except:
        pass # should auto-die when connection drops
    
    sys.stdin = sys.__stdin__
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__

# listening tasklet
def probe_accept(s):
    while True:
        try:
            c, a = s.accept()
            print a
            threading.Thread(target=probe_shell_thred, args=(c,)).start()
        except:
            pass
        
        stackless.schedule()

# k, go!
stackless.tasklet(probe_accept)(probe_sock)
