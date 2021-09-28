

hardpoints = [-2038.666, -411.709, -132.316, 			# lca1 x y z
		-2241.147, -408.195, -126.205, 					# lca2
		-2135, -600, -140, 								# lca3
		-2040.563, -416.249, -275.203, 					# uca1
		-2241.481, -417.314, -270.739, 					# uca2
		-2153, -578, -315, 								# uca3
		-2234.8, -411.45, -194.6, 						# tr1
		-2225, -582, -220,								# tr2
		-2143.6, -620.5, -220.07, 						# wcn
		-2143.6, -595.5, -219.34]                       # spn

test=list()
test.append(1),test.append(2)
print(test)

import ctypes as c
tenints = [1,2,3]

tenints_c = (c.c_int * 3)(*tenints)

for i in range (3):
	print(tenints_c[i])

lpy = tenints_c
print(lpy)

pyarr = [1, 2, 3, 4]
seq = c.c_int * len(pyarr)
arr = seq(*pyarr)
print()
print(type(seq))
print(type(arr))
print(seq)
print(arr)

for i in range (4):
	print(arr[i])


path = os.path.join(r"C:\dev\FS-BMK\bin\x64\Debug\mechanicsDLL.dll")

mydll = c.cdll.LoadLibrary(path)

mydll.optimisation_obj_res.argtypes = [
c.POINTER(c.c_float), 
c.c_float, c.c_float, c.c_float, c.c_float, c.c_float,
c.c_int, c.c_int, c.c_int,
c.c_float, c.c_float, 
c.c_int, c.c_int,
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
		-2143.6, -595.5, -219.34]                       # spn													
hardpoints_c = (c.c_float * len(hardpoints))(*hardpoints)

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
    
# OUTPUT PARAMETERS

outputParams =[]
outputParams_c = (c.c_float * 16)(*outputParams)

mydll.optimisation_obj_res(
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

print("OUTPUT PARAMETERS not a class ___________________________________-")
for i in range (16):
    #print(hardpoints_c[i])
    print(outputParams_c[i])