import ctypes as c
import os
#C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll
path = os.path.join(r"C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll")

print(path)
mydll = c.cdll.LoadLibrary(path)
mydll.optimisation_obj_res.restype = c.c_double

hardpoints = [-2038.666, -411.709, -132.316, 
		-2241.147, -408.195, -126.205, 
		-2135, -600, -140, 
		-2040.563, -416.249, -275.203, 
		-2241.481, -417.314, -270.739, 
		-2153, -578, -315, 
		-2234.8, -411.45, -194.6, 
		-2225, -582, -220,
		-2143.6, -620.5, -220.07, 
		-2143.6, -595.5, -219.34]

hardpoints_c = (c.c_float * len(hardpoints))(*hardpoints)


wRadiusin = c.c_float(210)
wVertin = c.c_float(30)
wSteerin = c.c_float(30)
vertIncrin = c.c_int(1)
steerIncrin = c.c_int(10)
precisionin = c.c_float(0.001)


a =mydll.optimisation_obj_res(
	hardpoints_c,
	wRadiusin,
wVertin ,
wSteerin ,
vertIncrin,
steerIncrin ,
precisionin
    )

print(type(a))
print("python: ", a)
print("python: ", a+10)

mydll.test_py.argtypes = [c.POINTER(c.c_double), c.POINTER(c.c_double)]

args = [1,2,3,4,6]
sol_list = []
args_c = (c.c_double * 5)(*args)
sol = (c.c_double * 5)(*sol_list)

mydll.test_py(args_c, sol)

for i in range (5):
    print(type(sol[i]))
