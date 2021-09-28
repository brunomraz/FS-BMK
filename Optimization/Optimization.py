import ctypes as c
import os
#C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll
path = os.path.join(r"C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll")

# wheel travel from rebound to bump, from downmost position w.r.t. chassis to upmost

print(path)
mydll = c.cdll.LoadLibrary(path)
mydll.optimisation_obj_res.restype = c.c_float

mydll.optimisation_obj_res.argtypes = [
	c.POINTER(c.c_float), 
	c.c_float,
	c.c_float,
	c.c_float,
	c.c_float,
	c.c_float,
	c.c_int,
	c.c_int,
	c.c_int,
	c.c_float,
	c.c_float,
	c.c_int,
	c.c_int,
	c.c_float,
	c.POINTER(c.c_float)]


hardpoints = [-2038.666, -411.709, -132.316, 			# lca1 x y z
		-2241.147, -408.195, -126.205, 					# lca2
		-2135, -600, -140, 								# lca3
		-2040.563, -416.249, -275.203, 					# uca1
		-2241.481, -417.314, -270.739, 					# uca2
		-2153, -578, -315, 								# uca3
		-2234.8, -411.45, -194.6, 						# tr1
		-2225, -582, -220,								# tr2
		-2143.6, -620.5, -220.07, 						# wcn
		-2143.6, -595.5, -219.34]						# spn
														#
hardpoints_c = (c.c_float * len(hardpoints))(*hardpoints)

# output params :
# 1  objective function
# 2  camber angle up 
# 3			down
# 4  toe angle up
# 5		    down
# 6  caster angle
# 7  roll centre height
# 8  caster trail
# 9  scrub radius
# 10 kingpin angle 
# 11 anti squat / anti dive   anti drive
# 12 anti rise / anti lift    anti brake
# 13 wheelbase change up 	
# 14		down
# 15 half track change up
# 16		down	

		
outputParams =[]
outputParams_c = (c.c_float * 16)(*outputParams)

wRadiusin = c.c_float(210)
wheelbase = c.c_float(1530)
cogHeight = c.c_float(300)
frontDriveBias = c.c_float(0)
frontBrakeBias = c.c_float(0.6)
suspPos = c.c_int(1) # 0 for front, 1 for rear
drivePos = c.c_int(1) # 0 for outboard, 1 for inboard
brakePos = c.c_int(0) # 0 for outboard, 1 for inboard

wVertin = c.c_float(30)
wSteerin = c.c_float(30)
vertIncrin = c.c_int(1)
steerIncrin = c.c_int(10)
precisionin = c.c_float(0.001)



a = mydll.optimisation_obj_res(
	hardpoints_c,
	wRadiusin,
	wheelbase,
	cogHeight,
	frontDriveBias,
	frontBrakeBias,
	suspPos,
	drivePos,
	brakePos,
	wVertin ,
	wSteerin ,
	vertIncrin,
	steerIncrin ,
	precisionin,
	outputParams_c
    )
# OUTPUT PARAMETERS
print("OUTPUT PARAMETERS")
for i in range (16-4):
    print(outputParams_c[i])



