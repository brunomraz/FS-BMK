import sympy as sp
from sympy import Point3D, simplify

# preslikavanje iz 3d u 2d po principu (x3d, y3d, z3d) -> (-y3d, z3d)

uca1x, uca1y, uca1z, uca2x, uca2y, uca2z, uca3x, uca3y, uca3z = sp.symbols("uca1[0], uca1[1], uca1[2], uca2[0], uca2[1], uca2[2], uca3[0], uca3[1], uca3[2]", real=True)
lca1x, lca1y, lca1z, lca2x, lca2y, lca2z, lca3x, lca3y, lca3z = sp.symbols("lca1[0], lca1[1], lca1[2], lca2[0], lca2[1], lca2[2], lca3[0], lca3[1], lca3[2]", real=True)
cpx, cpy, cpz = sp.symbols("cp[0], cp[1], cp[2]", real=True)  # contact patch
wcnx, wcny, wcnz = sp.symbols("wcn[0], wcn[1], wcn[2]", real=True)
# uca1x, uca1y, uca1z, uca2x, uca2y, uca2z, uca3x, uca3y, uca3z =[ 600, -300, 300, 700, -300, 300, 650, -600, 300]
# lca1x, lca1y, lca1z, lca2x, lca2y, lca2z, lca3x, lca3y, lca3z = [600, -300, 100, 700, -300, 100, 650, -600, 100]
# cpx, cpy, cpz = [650, -600, -10] # contact patch
# wcnx, wcny, wcnz = [650, -600, 200]

# uca1 = [600, -300, 300]
# uca2 = [700, -300, 300]
# uca3 = [650, -600, 300]
# lca1 = [600, -300, 100]
# lca2 = [700, -300, 100]
# lca3 = [650, -600, 100]
# cp = [650, -600, -10]  # contact patch
# wcn = [650, -600, 200]

xz_wcn_plane = sp.Plane(Point3D(0, cpy, 0), Point3D(1, cpy, 0), Point3D(0, cpy, 1))
uca_plane = sp.Plane(Point3D(uca1x, uca1y, uca1z), Point3D(uca2x, uca2y, uca2z), Point3D(uca3x, uca3y, uca3z))
lca_plane = sp.Plane(Point3D(lca1x, lca1y, lca1z), Point3D(lca2x, lca2y, lca2z), Point3D(lca3x, lca3y, lca3z))
yz_plane = sp.Plane(Point3D(0, 0, 1), Point3D(0, 1, 0), Point3D(0, 0, 0))
#
uca_line = sp.Plane.intersection(xz_wcn_plane, uca_plane)[0]
lca_line = sp.Plane.intersection(xz_wcn_plane, lca_plane)[0]

a_uca_formula = simplify(uca_line.direction_ratio[2] / uca_line.direction_ratio[0])
print("a_uca:")
print(a_uca_formula, end=2 * "\n")

b_uca_formula = simplify(sp.Plane.intersection(yz_plane, uca_line)[0][2])
print("b_uca:")
print(b_uca_formula, end=2 * "\n")

a_lca_formula = simplify(lca_line.direction_ratio[2] / lca_line.direction_ratio[0])
print("a_lca:")
print(a_lca_formula, end=2 * "\n")

b_lca_formula = simplify(sp.Plane.intersection(yz_plane, lca_line)[0][2])
print("b_lca:")
print(b_lca_formula, end=2 * "\n")

#

a_uca, b_uca, a_lca, b_lca = sp.symbols("a_uca, b_uca, a_lca, b_lca", real=True)

x, y = sp.symbols("x, y", real=True)
# #
plane_offset = sp.symbols("plane_offset", real=True)
y_line_2d = sp.Line2D(sp.Point2D(-plane_offset + cpx, 0), sp.Point2D(-plane_offset + cpx, 1))
# #

uca_line_2d_eq = sp.Line(a_uca * x + b_uca - y)
lca_line_2d_eq = sp.Line(a_lca * x + b_lca - y)
# #
# # provjera jesu li vektori paralelni, ako su slopeovi jednaki onda su paralelni
uca_line_2d_slope = uca_line_2d_eq.slope
lca_line_2d_slope = lca_line_2d_eq.slope
check_if_parallel = uca_line_2d_slope - lca_line_2d_slope
print("check if parallel")
print(simplify(check_if_parallel), end="\n\n")
# #
# # # # 1. slucaj
ref_pt_2d_x, ref_pt_2d_y = sp.symbols("ref_pt[0], ref_pt[2]", real=True)
parallel_line_2d = sp.Line2D(sp.Point2D(ref_pt_2d_x, ref_pt_2d_y), slope=uca_line_2d_slope)
rc_point_parallel_z_coord = parallel_line_2d.intersection(y_line_2d)[0][1]
# value to return in case of parallel control arms
rc_height_parallel = simplify(rc_point_parallel_z_coord - ref_pt_2d_y)
print("value to return in case of parallel control arms")
print(rc_height_parallel, end="\n\n")

# #
# # 2. slucaj
instant_roll_centre_2d_pt = uca_line_2d_eq.intersection(lca_line_2d_eq)[0]

instant_roll_centre_2d_pt_x = simplify(instant_roll_centre_2d_pt[0])
print("instant roll centre X")
print(instant_roll_centre_2d_pt_x, end="\n\n")
instant_roll_centre_2d_pt_y = simplify(instant_roll_centre_2d_pt[1])
print("instant roll centre Y")
print(instant_roll_centre_2d_pt_y, end="\n\n")
# #
cp_2d_ptx, cp_2d_pty = cpx, cpz
wcn_2d_ptx, wcn_2d_pty = wcnx, wcnz
irc_2d_ptx, irc_2d_pty = sp.symbols("irc_2d_ptx, irc_2d_pty", real=True)
#
# non_parallel_line_2d_cp = sp.Line2D(sp.Point2D(cpx, cpz), sp.Point2D(irc_2d_ptx, irc_2d_pty))
# non_parallel_line_2d_wcn = sp.Line2D(sp.Point2D(wcnx, wcnz), sp.Point2D(irc_2d_ptx, irc_2d_pty))
# rc_point_non_parallel_z_coord_cp = non_parallel_line_2d_cp.intersection(y_line_2d)[0][1]
# rc_point_non_parallel_z_coord_wcn = non_parallel_line_2d_wcn.intersection(y_line_2d)[0][1]
# # value to return in case of non parallel control arms
rc_height_non_parallel_cp = simplify(instant_roll_centre_2d_pt_y - cp_2d_pty)
rc_height_non_parallel_wcn = simplify(instant_roll_centre_2d_pt_y - wcn_2d_pty)
print("value to return in case of non parallel control arms CP")
print(rc_height_non_parallel_cp, end="\n\n")
print("value to return in case of non parallel control arms WCN")
print(rc_height_non_parallel_wcn, end="\n\n")
