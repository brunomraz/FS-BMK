import numpy as np
import formulae as f
import matplotlib as mpl
mpl.use('Qt4Agg')
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import ctypes as c
import os
# from numpy.random import uniform as rand
# from scipy.optimize import minimize
# import timeit
# import time

# output params :
# 0  objective function
# 1  camber angle down
# 2 			    up
# 3  toe angle down
# 4 		     up
# 5  caster angle
# 6  roll centre height
# 7  caster trail
# 8  scrub radius
# 9  kingpin angle
# 10 anti squat / anti dive drive
# 11 anti rise / anti lift  brake
# 12 half track change down
# 13 wheelbase change down
# 14 half track change up
# 15 wheelbase change up
# 16 distance lca3 to wcn-spn line
# 17 distance uca3 to wcn-spn line
# 18 distance tr2 to wcn-spn line
# 19 distance lca3 to plane with wcn-spn normal through wcn point
# 20 distance uca3 to plane with wcn-spn normal through wcn point
# 21 distance tr2 to plane with wcn-spn normal through wcn point 

# wheel travel from rebound to bump, from downmost position w.r.t. chassis to upmost


class Suspension():
    """creates quarter suspension defined by XYZ cs where X points front, Y to the right side and Z down"""
    path = os.path.abspath("../bin/x64/Release/mechanicsDLL.dll")
    #path = os.path.abspath("../bin/x64/Debug/mechanicsDLL.dll")


    mydll = c.cdll.LoadLibrary(path)

    mydll.optimisation_obj_res.argtypes = [
	c.POINTER(c.c_float), 
	c.c_float, c.c_float, c.c_float, c.c_float, c.c_float,
	c.c_int, c.c_int, c.c_int,
    c.c_float, c.c_float, 
    c.c_int, c.c_int,
	c.c_float,
	c.POINTER(c.c_float)]

    mydll.suspension_movement.argtypes = [
	c.POINTER(c.c_float), 
	c.c_float, c.c_float, c.c_float, c.c_float, c.c_float,
	c.c_int, c.c_int, c.c_int,
    c.c_float, c.c_float, 
    c.c_int, c.c_int,
	c.c_float,
	c.POINTER(c.c_float),
	c.POINTER(c.c_float)
    ]

    hardpoints2 = []															
    hardpoints2_c = (c.c_float * 15)(*hardpoints2)#c.c_float * 15

    hardpoints = []															
    hardpoints_c = c.c_float * 30

    wRadiusin = c.c_float(210)
    wheelbase = c.c_float(1530)
    cogHeight = c.c_float(300)
    frontDriveBias = c.c_float(0)
    frontBrakeBias = c.c_float(0.6)
    suspPos = c.c_int(1) # 0 for front, 1 for rear
    drivePos = c.c_int(1) # 0 for outboard, 1 for inboard
    brakePos = c.c_int(0) # 0 for outboard, 1 for inboard
    wVertin = c.c_float(-8)
    wSteerin = c.c_float(30)
    vertIncrin = c.c_int(1)
    steerIncrin = c.c_int(10)
    precisionin = c.c_float(0.001)
    
    # OUTPUT PARAMETERS

    outputParams =[]
    outputParams_c = (c.c_float * 22)(*outputParams)

    outputParams2 =[]
    outputParams2_c = (c.c_float * 11)(*outputParams2)


    # INPUT VALUES FOR OPTIMIZATION

    # boundaries
    lca1x_lo, lca1x_up, lca1y_lo, lca1y_up, lca1z_lo, lca1z_up = -2040,-2040,-490,-335,-180,-80
    lca2x_lo, lca2x_up, lca2y_lo, lca2y_up, lca2z_lo, lca2z_up = -2240,-2240,-490,-335,-180,-80
    lca3x_lo, lca3x_up, lca3y_lo, lca3y_up, lca3z_lo, lca3z_up = -2160,-2100,-630,-570,-170,-110

    uca1x_lo, uca1x_up, uca1y_lo, uca1y_up, uca1z_lo, uca1z_up = -2040,-2040,-490,-335,-325,-225
    uca2x_lo, uca2x_up, uca2y_lo, uca2y_up, uca2z_lo, uca2z_up = -2240,-2240,-490,-335,-325,-225
    uca3x_lo, uca3x_up, uca3y_lo, uca3y_up, uca3z_lo, uca3z_up = -2180,-2120,-610,-550,-345,-285

    tr1x_lo, tr1x_up, tr1y_lo, tr1y_up, tr1z_lo, tr1z_up = -2280,-2190,-485,-335,-245,-145
    tr2x_lo, tr2x_up, tr2y_lo, tr2y_up, tr2z_lo, tr2z_up = -2270,-2180,-610,-550,-280,-160

    wcnx_lo, wcnx_up, wcny_lo, wcny_up, wcnz_lo, wcnz_up = -2143.6, -2143.6, -620.5, -620.5, -220.07, -220.07
    spnx_lo, spnx_up, spny_lo, spny_up, spnz_lo, spnz_up = -2143.6, -2143.6, -595.5, -595.5, -219.34, -219.34


    
    # odreduju za koju vrijednost cambera se dobiju najbolje ocjene
    # zapravo se koristi samo _wantedCamberUp_uplim za gornju poziciju kotaca jer se to optimizira -2.65 , -0.97
    _wantedCamberDown_lolim = -2.7  # minimum wanted camber for wheel in top position
    _wantedCamberDown_uplim = -2.6  # maximum wanted camber for wheel in top position
    _wantedCamberUp_lolim = -1  # minimum wanted camber for wheel in top position
    _wantedCamberUp_uplim = -0.9 # maximum wanted camber for wheel in top position

    # granice toe anglea u gornjoj i donjoj poziciji kotaca -0.074, 0.048
    toe_lopos_lolim = -0.08
    toe_lopos_uplim = 0
    toe_uppos_lolim = 0
    toe_uppos_uplim = 0.05

    # wanted caster angle in ref pos (in degrees) 5.87
    _caster_angle_lolim = 4
    _caster_angle_uplim = 15

    # wanted roll centre height in ref pos 55.96
    _roll_centre_height_lolim = 50
    _roll_centre_height_uplim = 65

    # wanted caster trail in ref pos (in mm) 21.95
    _caster_trail_lolim = 10
    _caster_trail_uplim = 25

    # wanted scrub radius in ref pos (in mm) -10.3
    _scrub_radius_lolim = -15
    _scrub_radius_uplim = 8
    
    # wanted kingpin angle in ref pos (in degrees)  7.165
    _kingpin_angle_lolim = 3
    _kingpin_angle_uplim = 8
    
    # wanted anti lift- front suspension drive in percent 16.904
    wanted_anti_drive_lolim = 10
    wanted_anti_drive_uplim = 18

    # wanted anti dive- front suspension brake in percent 5.47
    wanted_anti_brake_lolim = 0
    wanted_anti_brake_uplim = 20

    # half track change lower position -4.93
    _half_track_change_downpos_lolim = -10
    _half_track_change_downpos_uplim = 0

    # wheelbase change lower position 0.77
    _wheelbase_change_downpos_lolim = -1.5
    _wheelbase_change_downpos_uplim = 1.5

    # half track change upper position 0.75
    _half_track_change_uppos_lolim = 0
    _half_track_change_uppos_uplim = 3
     
    # wheelbase change upper position -0.738
    _wheelbase_change_uppos_lolim = -1.5
    _wheelbase_change_uppos_uplim = 1.5

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije lca3 sa felgom (mm) 79.9
    _inside_wheel_free_radius_lca3_lolim = 60
    _inside_wheel_free_radius_lca3_uplim = 100

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije uca3 sa felgom (mm)
    # lca3,  i tr2 96.58
    _inside_wheel_free_radius_uca3_lolim = 60
    _inside_wheel_free_radius_uca3_uplim = 100

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije tr2 sa felgom (mm) 81.41
    _inside_wheel_free_radius_tr2_lolim = 60
    _inside_wheel_free_radius_tr2_uplim = 100

    # wanted minimum distance between plane defined by wcn and line wcn-spn and lca3 (mm) -22.828
    _wcn_lca3_distance_lolim = -100
    _wcn_lca3_distance_uplim = -20

    # wanted minimum distance between plane defined by wcn and line wcn-spn and uca3 (mm) -39.711
    _wcn_uca3_distance_lolim = -100
    _wcn_uca3_distance_uplim = -20

    # wanted minimum distance between plane defined by wcn and line wcn-spn and tr2 (mm) -38.485
    _wcn_tr2_distance_lolim = -100
    _wcn_tr2_distance_uplim = -20




    # odreduju koliko je siroko podrucje na kojem se dobivaju dobre ocjene, veca vrijednost znaci siljastiju funkciju
    _peakWidthUp = 100
    _peakWidthDown = 100
    _peakWidthUp_vector = 100000
    _peakWidthDown_vector = 100000
    # odreduje koliki je utjecaj objektne funkcije kod pomaka kotaca gore ili dolje
    _upWeightFactor = 0.5
    _downWeightFactor = 0.5

        
    def __init__(self, hps):  # hps is a list of hardpoints in order lca1, lca2, lca3, uca1, uca2, uca3, tr1, tr2, wcn, spn
        Suspension.hardpoints = hps


    def calculateMovement(self):
        hardpoints_c_arr = Suspension.hardpoints_c(*Suspension.hardpoints)
        Suspension.mydll.optimisation_obj_res(
	            hardpoints_c_arr,
	            Suspension.wRadiusin,
	            Suspension.wheelbase,
	            Suspension.cogHeight,
	            Suspension.frontDriveBias,
	            Suspension.frontBrakeBias,
	            Suspension.suspPos,
	            Suspension.drivePos,
	            Suspension.brakePos,
	            Suspension.wVertin ,
	            Suspension.wSteerin ,
	            Suspension.vertIncrin,
	            Suspension.steerIncrin ,
	            Suspension.precisionin,
	            Suspension.outputParams_c
                )

    def calculateMovement2(self):
        hardpoints_c_arr = Suspension.hardpoints_c(*Suspension.hardpoints)

        Suspension.mydll.suspension_movement(
	            hardpoints_c_arr,
	            Suspension.wRadiusin,
	            Suspension.wheelbase,
	            Suspension.cogHeight,
	            Suspension.frontDriveBias,
	            Suspension.frontBrakeBias,
	            Suspension.suspPos,
	            Suspension.drivePos,
	            Suspension.brakePos,
	            Suspension.wVertin ,
	            Suspension.wSteerin ,
	            Suspension.vertIncrin,
	            Suspension.steerIncrin ,
	            Suspension.precisionin,
	            Suspension.outputParams2_c,
	            Suspension.hardpoints2_c
                )


    def printResult(self):
        # OUTPUT PARAMETERS
        for i in range (17):
            #print(hardpoints_c[i])
            print(Suspension.outputParams_c[i])

    @classmethod
    def return_hps_and_parameters(cls):
        return Suspension.hardpoints + Suspension.outputParams_c[0:16]


def call_suspension_objective(hps):
    """funkcija prima kao listu sve varijabilne podatke, a u njoj samoj se zadaju konstante kao
    wcn, spn uca1x, uca2x, lca1x, lca2x
    input sadrzi redom slijedece brojeve:
    0-uca1y, 1-uca1z, 2-uca2y, 3-uca2z, 4-uca3x, 5-uca3y, 6-uca3z, 7-lca1y, 8-lca1z, 9-lca2y,
    10-lca2z, 11-lca3x, 12-lca3y, 13-lca3z, 14-tr1x, 15-tr1y, 16-tr1z, 17-tr2x, 18-tr2y, 19-tr2z"""

    s = Suspension([
        Suspension.lca1x_up, hps[0], hps[1],
        Suspension.lca2x_lo, hps[2], hps[3],
        hps[4], hps[5], hps[6],
        Suspension.uca1x_up, hps[7], hps[8],
        Suspension.uca2x_lo, hps[9], hps[10],
        hps[11], hps[12], hps[13],
        hps[14], hps[15], hps[16],
        hps[17], hps[18], hps[19],
        Suspension.wcnx_lo, Suspension.wcny_up, Suspension.wcnz_lo,
        Suspension.spnx_up, Suspension.spny_up, Suspension.spnz_up])
    s.calculateMovement()
    return Suspension.outputParams_c[0]


if __name__ == "__main__":
    print("suspension PARALLEL")
    suspension1 = Suspension([
        100, -500, 100,
        -100, -500, 100,
        0, -700, 100,
        100, -500, -100,
        -100, -500, -100,
        0, -700, -100,
        -100, -500, 0,
        -100, -700, 0,
        0, -700, 0,
        0, -650, 0

        ])            
    suspension1.calculateMovement()








    print("suspension output parameters___________")
    for i in range(22):
        print(Suspension.outputParams_c[i])
    print("suspension output parameters___________")
    print("suspension PARALLEL done")

    suspension = Suspension([
        -2038.666, -411.709, -132.316, 			# lca1 x y z
		-2241.147, -408.195, -126.205, 					# lca2
		-2135, -600, -140, 								# lca3
		-2040.563, -416.249, -275.203, 					# uca1
		-2241.481, -417.314, -270.739, 					# uca2
		-2153, -578, -315, 								# uca3
		-2234.8, -411.45, -194.6, 						# tr1
		-2225, -582, -220,								# tr2
		-2143.6, -620.5, -220.07, 						# wcn
		-2143.6, -595.5, -219.34])



    print("done creating class")
    suspension.calculateMovement()
    print("done calculating movement")

    print("suspension normal output parameters___________")
    for i in range(22):
        print(Suspension.outputParams_c[i])
    print("suspension output parameters___________")
    print("suspension normal done")

    print("suspension normal output parameters___________22222222222")
    suspension.calculateMovement2()
 
    for i in range(11):
        print(Suspension.outputParams2_c[i])
    print("suspension output parameters___________")
    print("suspension normal done")

    input("numeric_sol program finished, press any key")
