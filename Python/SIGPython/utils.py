import time


def measuretime(decimal=2):
    def decoratorfunction(f):
        def wrap(*args, **kwargs):
            time1 = time.time()
            result = f(*args, **kwargs)
            time2 = time.time()
            print('{:s} function took {:.{}f} ms'.format(
                f.__name__, ((time2-time1)*1000.0), decimal))
            return result
        return wrap
    return decoratorfunction
