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

# wheel travel from rebound to bump, from downmost position w.r.t. chassis to upmost





class Suspension():
    """creates quarter suspension defined by XYZ cs where X points front, Y to the right side and Z down"""
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
    wVertin = c.c_float(30)
    wSteerin = c.c_float(30)
    vertIncrin = c.c_int(1)
    steerIncrin = c.c_int(10)
    precisionin = c.c_float(0.001)
    
    # OUTPUT PARAMETERS

    outputParams =[]
    outputParams_c = (c.c_float * 16)(*outputParams)

    # INPUT VALUES FOR OPTIMIZATION

    # boundaries
    uca1x_lo, uca1x_up, uca1y_lo, uca1y_up, uca1z_lo, uca1z_up = np.array([546.963, 546.963, -500, -350, 200, 300])
    uca2x_lo, uca2x_up, uca2y_lo, uca2y_up, uca2z_lo, uca2z_up = np.array([747.881, 747.881, -500, -350, 200, 300])
    uca3x_lo, uca3x_up, uca3y_lo, uca3y_up, uca3z_lo, uca3z_up = np.array([620, 680, -620, -560, 270, 330])

    lca1x_lo, lca1x_up, lca1y_lo, lca1y_up, lca1z_lo, lca1z_up = np.array([545.066, 545.066, -500, -350, 50, 150])
    lca2x_lo, lca2x_up, lca2y_lo, lca2y_up, lca2z_lo, lca2z_up = np.array([747.547, 747.547, -500, -350, 50, 150])
    lca3x_lo, lca3x_up, lca3y_lo, lca3y_up, lca3z_lo, lca3z_up = np.array([620, 680, -630, -570, 80, 140])

    tr1x_lo, tr1x_up, tr1y_lo, tr1y_up, tr1z_lo, tr1z_up = np.array([690, 780, -500, -350, 150, 250])
    tr2x_lo, tr2x_up, tr2y_lo, tr2y_up, tr2z_lo, tr2z_up = np.array([690, 780, -620, -560, 170, 230])

    wcnx_lo, wcnx_up, wcny_lo, wcny_up, wcnz_lo, wcnz_up = np.array([650, 650, -620.5, -620.5, 200, 200])
    spnx_lo, spnx_up, spny_lo, spny_up, spnz_lo, spnz_up = np.array([650, 650, -595.5, -595.5, 199.27, 199.27])

    # granice toe anglea u gornjoj i donjoj poziciji kotaca
    toe_uppos_uplim = 0.05
    toe_uppos_lolim = 0
    toe_lopos_uplim = 0
    toe_lopos_lolim = -0.08

    # odreduju za koju vrijednost cambera se dobiju najbolje ocjene
    # zapravo se koristi samo _wantedCamberUp_uplim za gornju poziciju kotaca jer se to optimizira
    _wantedCamberUp_uplim = -3.82  # maximum wanted camber for wheel in top position
    _wantedCamberUp_lolim = -3.72  # minimum wanted camber for wheel in top position
    _wantedCamberDown_uplim = 0.25  # maximum wanted camber for wheel in top position
    _wantedCamberDown_lolim = 0.15  # minimum wanted camber for wheel in top position

    # wanted roll centre height in ref pos
    _roll_centre_height_uplim = 75
    _roll_centre_height_lolim = 65

    # wanted caster trail in ref pos (in mm)
    _caster_trail_uplim = 20
    _caster_trail_lolim = 10

    # wanted caster angle in ref pos (in degrees)
    _caster_angle_uplim = 15
    _caster_angle_lolim = 4

    # wanted kingpin angle in ref pos (in degrees)
    _kingpin_angle_uplim = 8
    _kingpin_angle_lolim = 3

    # wanted scrub radius in ref pos (in mm)
    _scrub_radius_uplim = 8
    _scrub_radius_lolim = -8

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije uca3 sa felgom (mm)
    # lca3,  i tr2
    _inside_wheel_free_radius_uca3_uplim = 100
    _inside_wheel_free_radius_uca3_lolim = 60

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije lca3 sa felgom (mm)
    _inside_wheel_free_radius_lca3_uplim = 100
    _inside_wheel_free_radius_lca3_lolim = 60

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije tr2 sa felgom (mm)
    _inside_wheel_free_radius_tr2_uplim = 100
    _inside_wheel_free_radius_tr2_lolim = 60

    # wanted minimum distance between plane defined by wcn and line wcn-spn and uca3 (mm)
    _wcn_uca3_distance_uplim = 25

    # wanted minimum distance between plane defined by wcn and line wcn-spn and lca3 (mm)
    _wcn_lca3_distance_uplim = 15

    # wanted minimum distance between plane defined by wcn and line wcn-spn and tr2 (mm)
    _wcn_tr2_distance_uplim = 25

    # half track change upper position
    _half_track_change_uppos_uplim = 3
    _half_track_change_uppos_lolim = 0

    # half track change lower position
    _half_track_change_downpos_uplim = 0
    _half_track_change_downpos_lolim = -10

    # wheelbase change upper position
    _wheelbase_change_uppos_uplim = 1.5
    _wheelbase_change_uppos_lolim = -1.5

    # wheelbase change lower position
    _wheelbase_change_downpos_uplim = 1.5
    _wheelbase_change_downpos_lolim = -1.5

    # wanted anti lift- front suspension drive in percent
    wanted_anti_lift_uplim = 18
    wanted_anti_lift_lolim = 10
    # wanted anti dive- front suspension brake in percent
    wanted_anti_dive_uplim = 20
    wanted_anti_dive_lolim = 0
    # wanted anti squat- rear suspension drive in percent
    wanted_anti_squat_uplim = 20
    wanted_anti_squat_lolim = 0
    # wanted anti rise- rear suspension brake in percent
    wanted_anti_rise_uplim = 20
    wanted_anti_rise_lolim = 0

    # odreduju koliko je siroko podrucje na kojem se dobivaju dobre ocjene, veca vrijednost znaci siljastiju funkciju
    _peakWidthUp = 100
    _peakWidthDown = 100
    _peakWidthUp_vector = 100000
    _peakWidthDown_vector = 100000
    # odreduje koliki je utjecaj objektne funkcije kod pomaka kotaca gore ili dolje
    _upWeightFactor = 0.5
    _downWeightFactor = 0.5


    
    def __init__(self, 
                 lca1x, lca1y, lca1z, 
                 lca2x, lca2y, lca2z, 
                 lca3x, lca3y, lca3z,
                 uca1x, uca1y, uca1z, 
                 uca2x, uca2y, uca2z, 
                 uca3x, uca3y, uca3z, 
                 tr1x, tr1y, tr1z,
                 tr2x, tr2y, tr2z,
                 wcnx, wcny, wcnz,
                 spnx, spny, spnz):
        Suspension.hardpoints.append(lca1x), Suspension.hardpoints.append(lca1y), Suspension.hardpoints.append(lca1z), 
        Suspension.hardpoints.append(lca2x), Suspension.hardpoints.append(lca2y), Suspension.hardpoints.append(lca2z), 
        Suspension.hardpoints.append(lca3x), Suspension.hardpoints.append(lca3y), Suspension.hardpoints.append(lca3z), 
        Suspension.hardpoints.append(uca1x), Suspension.hardpoints.append(uca1y), Suspension.hardpoints.append(uca1z), 
        Suspension.hardpoints.append(uca2x), Suspension.hardpoints.append(uca2y), Suspension.hardpoints.append(uca2z), 
        Suspension.hardpoints.append(uca3x), Suspension.hardpoints.append(uca3y), Suspension.hardpoints.append(uca3z), 
        Suspension.hardpoints.append(tr1x), Suspension.hardpoints.append(tr1y), Suspension.hardpoints.append(tr1z), 
        Suspension.hardpoints.append(tr2x), Suspension.hardpoints.append(tr2y), Suspension.hardpoints.append(tr2z), 
        Suspension.hardpoints.append(wcnx), Suspension.hardpoints.append(wcny), Suspension.hardpoints.append(wcnz), 
        Suspension.hardpoints.append(spnx), Suspension.hardpoints.append(spny), Suspension.hardpoints.append(spnz)
        print("done init")

    @classmethod
    def calculateMovement(cls):

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


    def printResult(self):
        # OUTPUT PARAMETERS
        for i in range (16):
            #print(hardpoints_c[i])
            print(Suspension.outputParams_c[i])


    def plotPoints(self):
        ### PLOT points
        fig = plt.figure()
        ax = fig.add_subplot(111, projection='3d')

        ax.scatter(self.tr2_travel_up.T[0], self.tr2_travel_up.T[1], self.tr2_travel_up.T[2])  # plots as points tierod2 travel from reference position upwards
        ax.scatter(self.tr2_travel_down.T[0], self.tr2_travel_down.T[1], self.tr2_travel_down.T[2])  # plots as points tierod2 travel from reference position downwards

        ax.scatter(self.lca_travel_up.T[0], self.lca_travel_up.T[1], self.lca_travel_up.T[2])  # plots as points LCA3 travel from reference position upwards
        ax.scatter(self.lca_travel_down.T[0], self.lca_travel_down.T[1], self.lca_travel_down.T[2])  # plots as points LCA3 travel from reference position downwards

        ax.scatter(self.uca_travel_up.T[0], self.uca_travel_up.T[1], self.uca_travel_up.T[2])  # plots as points UCA3 travel from reference position upwards
        ax.scatter(self.uca_travel_down.T[0], self.uca_travel_down.T[1], self.uca_travel_down.T[2])  # plots as points UCA3 travel from reference position downwards

        ax.scatter(self.wcn_travel_up.T[0], self.wcn_travel_up.T[1], self.wcn_travel_up.T[2])  # plots as points WCN travel from reference position upwards
        ax.scatter(self.wcn_travel_down.T[0], self.wcn_travel_down.T[1], self.wcn_travel_down.T[2])  # plots as points WCN travel from reference position downwards

        ax.scatter(self.spn_travel_up.T[0], self.spn_travel_up.T[1], self.spn_travel_up.T[2])  # plots as points Spindle travel from reference position upwards
        ax.scatter(self.spn_travel_down.T[0], self.spn_travel_down.T[1], self.spn_travel_down.T[2])  # plots as points Spindle travel from reference position downwards

        ax.scatter(self.cp_up.T[0], self.cp_up.T[1], self.cp_up.T[2])  # plots as points Spindle travel from reference position upwards
        ax.scatter(self.cp_down.T[0], self.cp_down.T[1], self.cp_down.T[2])  # plots as points Spindle travel from reference position downwards


        # plots as lines LCA, UCA and TR links in reference position
        ax.plot([self.lca1[0], self.lca2[0], self.lca3[0]],
                [self.lca1[1], self.lca2[1], self.lca3[1]],
                [self.lca1[2], self.lca2[2], self.lca3[2]])
        ax.plot([self.uca1[0], self.uca2[0], self.uca3[0]],
                [self.uca1[1], self.uca2[1], self.uca3[1]],
                [self.uca1[2], self.uca2[2], self.uca3[2]])
        ax.plot([self.tr1[0], self.tr2[0]],
                [self.tr1[1], self.tr2[1]],
                [self.tr1[2], self.tr2[2]])
        ax.plot([self.lca2[0]], [self.lca2[1]], [self.lca2[2]], "bo")

        # plots as lines XYZ axes
        # ax.plot((0, 0), (0, 0), (0, 100), '-b', label='z-axis')
        # ax.plot((0, 0), (0, 100), (0, 0), '-g', label='y-axis')
        # ax.plot((0, 100), (0, 0), (0, 0), '-r', label='x-axis')

        # bounding box for equal scaling
        # ax.scatter((0,2000,0,0),(0,0,-2000,0),(0,0,0,2000))

        ax.legend()
        plt.show()
    #



"""objective function"""


def call_suspension_objective(hps):
    """funkcija prima kao listu sve varijabilne podatke, a u njoj samoj se zadaju konstante kao
    wcn, spn uca1x, uca2x, lca1x, lca2x
    input sadrzi redom slijedece brojeve:
    0-uca1y, 1-uca1z, 2-uca2y, 3-uca2z, 4-uca3x, 5-uca3y, 6-uca3z, 7-lca1y, 8-lca1z, 9-lca2y,
    10-lca2z, 11-lca3x, 12-lca3y, 13-lca3z, 14-tr1x, 15-tr1y, 16-tr1z, 17-tr2x, 18-tr2y, 19-tr2z"""

    s = Suspension(uca1=np.array([Suspension.uca1x_up, hps[0], hps[1]]),
                   uca2=np.array([Suspension.uca2x_lo, hps[2], hps[3]]),
                   uca3=np.array([hps[4], hps[5], hps[6]]),
                   lca1=np.array([Suspension.lca1x_up, hps[7], hps[8]]),
                   lca2=np.array([Suspension.lca2x_lo, hps[9], hps[10]]),
                   lca3=np.array([hps[11], hps[12], hps[13]]),
                   tr1=np.array([hps[14], hps[15], hps[16]]),
                   tr2=np.array([hps[17], hps[18], hps[19]]),
                   wcn=np.array([Suspension.wcnx_lo, Suspension.wcny_up, Suspension.wcnz_lo]),
                   spn=np.array([Suspension.spnx_up, Suspension.spny_up, Suspension.spnz_up]))
    s.calculateMovement()
    s.camber_score()
    # s.plotPoints()
    # print(f"s objective sum: {s.objectiveSum}")
    return s.objectiveSum


def call_suspension_check(hps):
    """funkcija prima kao listu sve varijabilne podatke, a u njoj samoj se zadaju konstante kao
    wcn, spn uca1x, uca2x, lca1x, lca2x
    input sadrzi redom slijedece brojeve:
    0-uca1y, 1-uca1z, 2-uca2y, 3-uca2z, 4-uca3x, 5-uca3y, 6-uca3z, 7-lca1y, 8-lca1z, 9-lca2y,
    10-lca2z, 11-lca3x, 12-lca3y, 13-lca3z, 14-tr1x, 15-tr1y, 16-tr1z, 17-tr2x, 18-tr2y, 19-tr2z"""

    s = Suspension(uca1=np.array([Suspension.uca1x_up, hps[0], hps[1]]),
                   uca2=np.array([Suspension.uca2x_lo, hps[2], hps[3]]),
                   uca3=np.array([hps[4], hps[5], hps[6]]),
                   lca1=np.array([Suspension.lca1x_up, hps[7], hps[8]]),
                   lca2=np.array([Suspension.lca2x_lo, hps[9], hps[10]]),
                   lca3=np.array([hps[11], hps[12], hps[13]]),
                   tr1=np.array([hps[14], hps[15], hps[16]]),
                   tr2=np.array([hps[17], hps[18], hps[19]]),
                   wcn=np.array([Suspension.wcnx_lo, Suspension.wcny_up, Suspension.wcnz_lo]),
                   spn=np.array([Suspension.spnx_up, Suspension.spny_up, Suspension.spnz_up]))
    s.calculateMovement()
    # s.plotPoints()
    return [f"camber up: {s.camberUp[-1].round(3)}° camber down: {s.camberDown[-1].round(3)}°",
            s.camberUp[-1].round(3),
            s.camberDown[-1].round(3),
            np.concatenate([
                s.uca1, s.uca2, s.uca3,
                s.lca1, s.lca2, s.lca3,
                s.tr1, s.tr2,
                s.wcn, s.spn,
                [s.camberUp[-1], s.camberDown[-1],
                 s.toeUp[-1], s.toeDown[-1],
                 s.ref_pos_roll_centre_height,
                 s.caster_trail_ref_pos, s.caster_angle_ref_pos,
                 s.kingpin_angle_ref_pos, s.scrub_radius_ref_pos,
                 s.half_track_change_up, s.half_track_change_down,
                 s.wheelbase_change_up, s.wheelbase_change_down,
                 s.anti_drive_feature, s.anti_brake_feature]]).round(3)

            ]




if __name__ == "__main__":
    # suspension1 = Suspension(uca1=np.array([600, -300, 300]),
    #                          uca2=np.array([700, -300, 300]),
    #                          uca3=np.array([650, -600, 300]),
    #                          lca1=np.array([600, -300, 100]),
    #                          lca2=np.array([700, -300, 100]),
    #                          lca3=np.array([650, -600, 100]),
    #                          tr1=np.array([700, -300, 200]),
    #                          tr2=np.array([700, -600, 200]),
    #                          wcn=np.array([650, -600, 200]),
    #                          spn=np.array([650, -550, 200]))
    # print(suspension1)

    suspension = Suspension(-2038.666, -411.709, -132.316, 			# lca1 x y z
		-2241.147, -408.195, -126.205, 					# lca2
		-2135, -600, -140, 								# lca3
		-2040.563, -416.249, -275.203, 					# uca1
		-2241.481, -417.314, -270.739, 					# uca2
		-2153, -578, -315, 								# uca3
		-2234.8, -411.45, -194.6, 						# tr1
		-2225, -582, -220,								# tr2
		-2143.6, -620.5, -220.07, 						# wcn
		-2143.6, -595.5, -219.34)
    print(suspension.hardpoints)
    suspension.calculateMovement()
    suspension.printResult()



    input("zavrsio program, pritisni bilokoju tpiku")
