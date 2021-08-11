import ctypes as c
import os
#C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll
path = os.path.join(r"C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll")

print(path)
mydll = c.cdll.LoadLibrary(path)
mydll.optimisation_obj_res.restype = c.c_double

a =mydll.optimisation_obj_res()

print(type(a))
print("python: ", a)
print("python: ", a+10)

