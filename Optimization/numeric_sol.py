import numpy as np
import ctypes as c
import os

# output params :
# 0 objective function
# 1 camber angle down
# 2 up
# 3 toe angle down
# 4 up
# 5 caster angle
# 6 roll centre height
# 7 caster trail
# 8 scrub radius
# 9 kingpin angle
# 10 anti squat / anti dive drive
# 11 anti rise / anti lift brake
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

# wheel travel from rebound to bump, from downmost position w.r.t.  chassis to
# upmost
class Suspension():
    """creates quarter suspension defined by XYZ cs where X points front, Y to the right side and Z down"""

    
    wheel_radius = 210
    wheelbase = 1530
    cog_height = 300
    drive_bias = 1
    brake_bias = 0.4
    suspension_position = 1 # 0 for front, 1 for rear
    drive_position = 1 # 0 for outboard, 1 for inboard
    brake_position = 0 # 0 for outboard, 1 for inboard
    vertical_movement = 30
    steering_movement = 10
    vert_incr = 1
    steer_incr = 1
    precision = 0.001
       
    # OUTPUT PARAMETERS
    # optimisation output parameters
    output_params_optimisation = []
    output_params_optimisation_c = (c.c_float * 21)(*output_params_optimisation)

    # suspension movement output parameters

    camberAngle = []															
    camberAngle_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*camberAngle)
    toeAngle = []															
    toeAngle_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*toeAngle)
    casterAngle = []															
    casterAngle_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*casterAngle)
    rcHeight = []															
    rcHeight_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*rcHeight)
        
    casterTrail = []															
    casterTrail_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*casterTrail)
    scrubRadius = []															
    scrubRadius_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*scrubRadius)
    kingpinAngle = []															
    kingpinAngle_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*kingpinAngle)
    antiDrive = []															
    antiDrive_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*antiDrive)

    antiBrake = []															
    antiBrake_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*antiBrake)
    halfTrackChange = []															
    halfTrackChange_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*halfTrackChange)
    wheelbaseChange = []															
    wheelbaseChange_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*wheelbaseChange)

    const_output_params_movement = []
    const_output_params_movement_c = (c.c_float * 6)(*const_output_params_movement)

    # hps boundaries
    hps_boundaries = [-2038.666,-490,-335,-180,-80,     # 0 lca1x_opt, 1 lca1y_lo, 2 lca1y_up, 3 lca1z_lo, 4 lca1z_up
        -2241.147,-490,-335,-180,-80,     # 5 lca2x_opt, 6 lca2y_lo, 7 lca2y_up, 8 lca2z_lo, 9 lca2z_up
        -2160,-2100,-630,-570,-170,-110,  # 10 lca3x_lo, 11 lca3x_up, 12 lca3y_lo, 13 lca3y_up, 14 lca3z_lo, 15
                                          # lca3z_up
                                           
        -2040.563,-490,-335,-325,-225,    # 16 uca1x_opt, 17 uca1y_lo, 18 uca1y_up, 19 uca1z_lo, 20 uca1z_up
        -2241.481,-490,-335,-325,-225,    # 21 uca2x_opt, 22 uca2y_lo, 23 uca2y_up, 24 uca2z_lo, 25 uca2z_up
        -2180,-2120,-610,-550,-345,-285,  # 26 uca3x_lo, 27 uca3x_up, 28 uca3y_lo, 29 uca3y_up, 30 uca3z_lo, 31
                                          # uca3z_up
                                           
        -2280,-2190,-485,-335,-245,-145,  # 32 tr1x_lo, 33 tr1x_up, 34 tr1y_lo, 35 tr1y_up, 36 tr1z_lo, 37 tr1z_up
        -2270,-2180,-610,-550,-280,-160,  # 38 tr2x_lo, 39 tr2x_up, 40 tr2y_lo, 41 tr2y_up, 42 tr2z_lo, 43 tr2z_up
                                          
        -2143.6, -620.5, -220.07,         # indices: 44, 45, 46, # wcnx_opt, wcny_opt, wcnz_opt
        -2143.6, -595.5, -219.34          # indices: 47, 48, 49 # spnx_opt, spny_opt, spnz_opt
        ]
    
    # INPUT FOR OPTIMISATION.PY
    # derived suspension characteristics boundaries
    characteristics_boundaries = [-2.7, -2.6,        # indices: 0, 1, # camber down pos lo hi lim
        -1, -0.9,          # indices: 2, 3, # camber up pos lo hi lim
        -0.08, 0,          # indices: 4, 5, # toe down pos lo hi lim
        0, 0.05,           # indices: 6, 7, # toe up pos lo hi lim
        4, 15,             # indices: 8, 9, # caster angle lo hi lim
        50, 65,            # indices: 10, 11, # roll centre height lo hi lim
        10, 25,            # indices: 12, 13, # caster trail lo hi lim
        -15, 8,            # indices: 14, 15, # scrub radius lo hi lim
        3, 8,              # indices: 16, 17, # kingpin angle lo hi lim
        10, 18,            # indices: 18, 19, # anti drive lo hi lim
        0, 20,             # indices: 20, 21, # anti brake lo hi lim
        -10, 0,            # indices: 22, 23, # half track change down pos lo hi lim
        0, 3,              # indices: 24, 25, # half track change up pos lo hi lim
        -1.5, 1.5,         # indices: 26, 27, # wheelbase change down pos lo hi lim
        -1.5, 1.5,         # indices: 28, 29, # wheelbase change up pos lo hi lim
        60, 100,           # indices: 30, 31, # inside wheel free radius LCA3 lo hi lim
        60,100,            # indices: 32, 33, # inside wheel free radius UCA3 lo hi lim
        60,100,            # indices: 34, 35, # inside wheel free radius TR2 lo hi lim
       -100,-20,           # indices: 36, 37, # minimum distance between plane defined by wcn
                           # and line wcn-spn and LCA3 (mm) lo hi lim
       -100,-20,           # indices: 38, 39, # minimum distance between plane defined by wcn
                           # and line wcn-spn and UCA3 (mm) lo hi lim
       -100,-20,           # indices: 40, 41 # minimum distance between plane defined by wcn
                           # and line wcn-spn and TR2 (mm) lo hi lim
        ]

        # determines objective function shape
    peak_width_values = [
        #10, 10, 10, 10, 10, 10, 10, 10, 10,10,10,10, 10, 10, 10,10, 10,10, 10, 10, 10 
        0.10000000149011612 ,
        0.10000000149011612 ,
        10,
        10,
        0.10000000149011612 ,
        10,
        200000,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10,
        10
        ]
    
    peak_flatness_values = [
        #2, 2, 2, 2, 6, 2, 6, 2, 2,2,2,2, 2, 2, 2,2, 2,2, 2, 2, 2 
        2,
        2,
        2,
        2,
        3,
        2,
        6,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2

        
        ]
        # INPUT FOR DLL FILE OBJECTIVE FUNCTION
    characteristics_target_values = [
        #-2.65, -0.95, 0, 0, 4, 50, 10, -15, 3, 10, 10, 0, 0, 0, 0, 70, 70, 70, -80, -80, -80
        -2.6500000953674316 ,
        -0.949999988079071 ,
        0,
        0,
        7,
        50,
        15,
        -7,
        4,
        18,
        10,
        0,
        0,
        0,
        0,
        100,
        100,
        100,
        -20,
        -20,
        -20

        ]
    # INPUT FOR DLL FILE OBJECTIVE FUNCTION
    characteristics_weight_factors = [
        #1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        0.4032258093357086 ,
        0.4032258093357086 ,
        0.02016128972172737 ,
        0.02016128972172737 ,
        0.012096770107746124 ,
        0.008064515888690948 ,
        0.02016128972172737 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948 ,
        0.008064515888690948

    ]

    obj_func_res = 0

    obj_func_res_c = c.c_float(obj_func_res)

    suspension_position_c = c.c_int(suspension_position) 
    wheel_radius_c = c.c_float(wheel_radius)
    wheelbase_c = c.c_float(wheelbase)
    cog_height_c = c.c_float(cog_height)
    drive_bias_c = c.c_float(drive_bias)
    brake_bias_c = c.c_float(brake_bias)
    drive_position_c = c.c_int(drive_position)
    brake_position_c = c.c_int(brake_position)
    vertical_movement_c = c.c_float(vertical_movement)
    
    
    steering_movement_c = c.c_float(steering_movement)
    vert_incr_c = c.c_int(vert_incr)
    steer_incr_c = c.c_int(steer_incr)

    precision_c = c.c_float(precision)
    
    
    lca3_moved = []															
    lca3_moved_c = (c.c_float * (vert_incr * 2 + 1))(*lca3_moved)
    
    uca3_moved = []															
    uca3_moved_c = (c.c_float * (vert_incr * 2 + 1))(*uca3_moved)

    tr1_moved = []															
    tr1_moved_c = (c.c_float * (steer_incr * 2 + 1))(*tr1_moved)

    tr2_moved = []															
    tr2_moved_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*tr2_moved)

    wcn_moved = []															
    wcn_moved_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*wcn_moved)

    spn_moved = []															
    spn_moved_c = (c.c_float * ((vert_incr * 2 + 1) * (steer_incr * 2 + 1)))(*spn_moved)



    hardpoints = []															
    hardpoints_c = c.c_float * 30
    
    characteristics_target_values_c = (c.c_float * 21)(*characteristics_target_values)
    characteristics_weight_factors_c = (c.c_float * 21)(*characteristics_weight_factors)
    peak_width_values_c = (c.c_float * 21)(*peak_width_values)
    peak_flatness_values_c = (c.c_float * 21)(*peak_flatness_values)


    # path to dll for calculating kinematics and parameters
    path = os.path.abspath(r"C:\dev\FS-BMK\bin\x64\Release\mechanicsDLL.dll")

    mydll = c.cdll.LoadLibrary(path)

    mydll.optimisation_obj_res.argtypes = [c.POINTER(c.c_float),
	    type(suspension_position_c), 
        type(wheel_radius_c), 
        type(wheelbase_c), 
        type(cog_height_c),
        type(drive_bias_c), 
        type(brake_bias_c),
        type(drive_position_c), 
        type(brake_position_c),
        type(vertical_movement_c), 
        type(peak_width_values_c),
        type(peak_flatness_values_c),
        type(steering_movement_c), 
        type(vert_incr_c), 
        type(steer_incr_c),
	    type(precision_c),
        type(characteristics_target_values_c),
        type(characteristics_weight_factors_c),
        c.POINTER(type(obj_func_res_c)),
	    c.POINTER(type(output_params_optimisation_c))]

    mydll.suspension_movement.argtypes = [c.POINTER(c.c_float),
        type(wheel_radius_c), 
        type(wheelbase_c), 
        type(cog_height_c), 
        type(drive_bias_c), 
        type(brake_bias_c),
	    type(suspension_position_c), 
        type(drive_position_c), 
        type(brake_position_c),
        type(vertical_movement_c), 
        type(steering_movement_c), 
        type(vert_incr_c), 
        type(steer_incr_c),
	    type(precision_c),

	    c.POINTER(type(camberAngle_c)),
	    c.POINTER(type(toeAngle_c)),
	    c.POINTER(type(casterAngle_c)),
	    c.POINTER(type(rcHeight_c)),
	    c.POINTER(type(casterTrail_c)),
	    c.POINTER(type(scrubRadius_c)),
	    c.POINTER(type(kingpinAngle_c)),
	    c.POINTER(type(antiDrive_c)),
	    c.POINTER(type(antiBrake_c)),
	    c.POINTER(type(halfTrackChange_c)),
	    c.POINTER(type(wheelbaseChange_c)),
	    c.POINTER(type(const_output_params_movement_c)),

	    c.POINTER(type(lca3_moved_c)),
	    c.POINTER(type(uca3_moved_c)),
	    c.POINTER(type(tr1_moved_c)),
	    c.POINTER(type(tr2_moved_c)),
	    c.POINTER(type(wcn_moved_c)),
	    c.POINTER(type(spn_moved_c))

    ]

    def __init__(self, hps):  # hps is a list of hardpoints in order lca1, lca2, lca3, uca1, uca2, uca3,
                              # tr1, tr2, wcn, spn
        Suspension.hardpoints = hps


    def calculateOptimisationMovement(self):
        hardpoints_c_arr = Suspension.hardpoints_c(*Suspension.hardpoints)
        Suspension.mydll.optimisation_obj_res(
            Suspension.hardpoints_c(*Suspension.hardpoints),
            #hardpoints_c_arr,
	        Suspension.suspension_position_c,
	        Suspension.wheel_radius_c,
	        Suspension.wheelbase_c,
	        Suspension.cog_height_c,
	        Suspension.drive_bias_c,
	        Suspension.brake_bias_c,
	        Suspension.drive_position_c,
	        Suspension.brake_position_c,
	        Suspension.vertical_movement_c ,
	        Suspension.peak_width_values_c,
	        Suspension.peak_flatness_values_c,
	        Suspension.steering_movement_c ,
	        Suspension.vert_incr_c,
	        Suspension.steer_incr_c,
	        Suspension.precision_c,
	        Suspension.characteristics_target_values_c,
	        Suspension.characteristics_weight_factors_c,
            Suspension.obj_func_res_c,
	        Suspension.output_params_optimisation_c)

    def calculateMovement(self):
        # hardpoints_c_arr = Suspension.hardpoints_c(*Suspension.hardpoints)

        Suspension.mydll.suspension_movement(
            Suspension.hardpoints_c(*Suspension.hardpoints),
            # hardpoints_c_arr,
	        Suspension.wheel_radius_c,
	        Suspension.wheelbase_c,
	        Suspension.cog_height_c,
	        Suspension.drive_bias_c,
	        Suspension.brake_bias_c,
	        Suspension.suspension_position_c,
	        Suspension.drive_position_c,
	        Suspension.brake_position_c,
	        Suspension.vertical_movement_c ,
	        Suspension.steering_movement_c ,
	        Suspension.vert_incr_c,
	        Suspension.steer_incr_c ,
	        Suspension.precision_c,

	        Suspension.camberAngle_c,
	        Suspension.toeAngle_c,
	        Suspension.casterAngle_c,
	        Suspension.rcHeight_c,
	        Suspension.casterTrail_c,
	        Suspension.scrubRadius_c,
	        Suspension.kingpinAngle_c,
	        Suspension.antiDrive_c,
	        Suspension.antiBrake_c,
	        Suspension.halfTrackChange_c,
	        Suspension.wheelbaseChange_c,
	        Suspension.const_output_params_movement_c,

	        Suspension.lca3_moved_c,
	        Suspension.uca3_moved_c,
	        Suspension.tr1_moved_c,
	        Suspension.tr2_moved_c,
	        Suspension.wcn_moved_c,
	        Suspension.spn_moved_c
            )


    @classmethod
    def return_hps_and_parameters(cls):
        # return Suspension.hardpoints +
        # Suspension.output_params_optimisation_c[0:16]
        return Suspension.hardpoints + Suspension.output_params_optimisation_c[:]


def call_suspension_objective(hps):
    s = Suspension([
        Suspension.hps_boundaries[0], hps[0], hps[1],
        Suspension.hps_boundaries[5], hps[2], hps[3],
        hps[4], hps[5], hps[6],
        Suspension.hps_boundaries[16], hps[7], hps[8],
        Suspension.hps_boundaries[21], hps[9], hps[10],
        hps[11], hps[12], hps[13],
        hps[14], hps[15], hps[16],
        hps[17], hps[18], hps[19],
        Suspension.hps_boundaries[44], Suspension.hps_boundaries[45], Suspension.hps_boundaries[46],
        Suspension.hps_boundaries[47], Suspension.hps_boundaries[48], Suspension.hps_boundaries[49]])
    s.calculateOptimisationMovement()
    return Suspension.obj_func_res_c.value


if __name__ == "__main__":
    #print("suspension PARALLEL")
    #suspension1 = Suspension([
    #    100, -500, 100,
    #    -100, -500, 100,
    #    0, -700, 100,
    #    100, -500, -100,
    #    -100, -500, -100,
    #    0, -700, -100,
    #    -100, -500, 0,
    #    -100, -700, 0,
    #    0, -700, 0,
    #    0, -650, 0
    #
    #    ])
    #suspension1.calculateOptimisationMovement()
    
    #print("suspension output parameters___________")
    #for i in range(22):
    #    print(Suspension.output_params_optimisation_c[i])
    #print("suspension output parameters___________")
    #print("suspension PARALLEL done")

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
    #suspension.calculateOptimisationMovement()
    #print("done calculating movement")

    #print("Suspension optimisation output parameters:")
    #for i in range(21):
    #    print(Suspension.output_params_optimisation_c[i])

    #print("obj func result:")
    #print(Suspension.obj_func_res_c)

    print("suspension movement output parameters:")
    suspension.calculateMovement()
 
    for i in range(6):
        print(Suspension.const_output_params_movement_c[i])

    input("numeric_sol program finished, press any key")
