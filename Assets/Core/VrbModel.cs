using System.Collections;
using System.Collections.Generic;

namespace vrb
{
	public class VrbPoint
	{
		public static List<VrbPoint> all = new List<VrbPoint>();

		private int index; // 点的索引
		public int getIndex()
		{
			return index;
		}

		public float x;
		public float y;
		public float z;

		// 构造函数，每个实例都要将自己加入静态数组all里
		public VrbPoint()
		{
			addToStatic();
		}
		public VrbPoint(float xx, float yy, float zz)
		{
			x = xx;
			y = yy;
			z = zz;
			addToStatic();
		}
		// 加入类的静态统计中，并创建索引
		private void addToStatic()
		{
			index = all.Count;
			all.Add(this);
		}
	}

	// 三角形面片，记录顶点索引
	public class VrbTriangle
	{
		public static List<VrbTriangle> all = new List<VrbTriangle>();

		private int index;
		public int getIndex()
		{
			return index;
		}

		// 保存所有的点
		public VrbPoint[] vertices = new VrbPoint[3];

		// 构造函数，每个实例都要将自己加入静态数组all里
		public VrbTriangle()
		{
			addToStatic();
		}
		public VrbTriangle(VrbPoint p1, VrbPoint p2, VrbPoint p3)
		{
			vertices[0] = p1;
			vertices[1] = p2;
			vertices[2] = p3;
			addToStatic();
		}
		// 加入类的静态统计中，并创建索引
		private void addToStatic()
		{
			index = all.Count;
			all.Add(this);
		}
	}

	// 多面体的面，记录三角面片的索引
	public class VrbFace
	{
		public static List<VrbFace> all = new List<VrbFace>();

		private int index;
		public int getIndex()
		{
			return index;
		}

		// 所有三角面片
		public List<VrbTriangle> triangles = new List<VrbTriangle>();

		// 构造函数，每个实例都要将自己加入静态数组all里
		public VrbFace()
		{
			addToStatic();
		}
		public VrbFace(List<VrbTriangle> _triangles)
		{
			triangles = _triangles;
			addToStatic();
		}
		public VrbFace(VrbTriangle t0)
		{
			triangles.Add(t0);
			addToStatic();
		}
		public VrbFace(VrbTriangle t0, VrbTriangle t1)
		{
			triangles.Add(t0);
			triangles.Add(t1);
			addToStatic();
		}
		// 加入类的静态统计中，并创建索引
		private void addToStatic()
		{
			index = all.Count;
			all.Add(this);
		}


		// 其它接口
		public List<VrbPoint> getFacePoints()
		{
			List<VrbPoint> points = new List<VrbPoint>();
			foreach (VrbTriangle t in triangles)
			{
				for (int pIndex = 0; pIndex < 3; pIndex++)
				{
					if (points.IndexOf(t.vertices[pIndex]) == -1)
					{
						points.Add(t.vertices[pIndex]);
					}
				}
			}
			return points;
		}
	}

	public class VrbObject
	{
		public static List<VrbObject> all = new List<VrbObject>();
		public int x, y, z; // 位置
		public int rx, ry, rz; // 旋转
		public int sx, sy, sz; // 三方向scale

		public List<VrbFace> faces = new List<VrbFace>();

		public VrbObject()
		{
			all.Add(this);
		}

		public VrbObject(List<VrbFace> _faces)
		{
			faces = _faces;
			all.Add(this);
		}
	}

	public static class VrbModel
	{
		public static void createQuad()
		{
			VrbPoint p0 = new VrbPoint(100, 200, 0);
			VrbPoint p1 = new VrbPoint(100, 0, 0);
			VrbPoint p2 = new VrbPoint(-100, 0, 0);
			VrbPoint p3 = new VrbPoint(-100, 200, 0);

			VrbTriangle t0 = new VrbTriangle(p0, p1, p2);
			VrbTriangle t1 = new VrbTriangle(p0, p2, p3);

			VrbFace f0 = new VrbFace(t0, t1);

			List<VrbFace> fList = new List<VrbFace>();
			fList.Add(f0);
			new VrbObject(fList);
		}
		// 前三个是位置坐标，后三个是大小
		public static void spawnCube(float xp, float yp, float zp, float xl, float yl, float zl)
		{
			VrbPoint p0 = new VrbPoint(xp + xl / 2, yp + yl / 2, zp + zl / 2);
			VrbPoint p1 = new VrbPoint(xp + xl / 2, yp + yl / 2, zp - zl / 2);
			VrbPoint p2 = new VrbPoint(xp + xl / 2, yp - yl / 2, zp + zl / 2);
			VrbPoint p3 = new VrbPoint(xp + xl / 2, yp - yl / 2, zp - zl / 2);
			VrbPoint p4 = new VrbPoint(xp - xl / 2, yp + yl / 2, zp + zl / 2);
			VrbPoint p5 = new VrbPoint(xp - xl / 2, yp + yl / 2, zp - zl / 2);
			VrbPoint p6 = new VrbPoint(xp - xl / 2, yp - yl / 2, zp + zl / 2);
			VrbPoint p7 = new VrbPoint(xp - xl / 2, yp - yl / 2, zp - zl / 2);

			VrbTriangle t0 = new VrbTriangle(p0, p1, p3);
			VrbTriangle t1 = new VrbTriangle(p0, p2, p3);
			VrbTriangle t2 = new VrbTriangle(p0, p1, p4);
			VrbTriangle t3 = new VrbTriangle(p1, p4, p5);
			VrbTriangle t4 = new VrbTriangle(p1, p3, p5);
			VrbTriangle t5 = new VrbTriangle(p3, p5, p7);
			VrbTriangle t6 = new VrbTriangle(p4, p5, p6);
			VrbTriangle t7 = new VrbTriangle(p5, p6, p7);
			VrbTriangle t8 = new VrbTriangle(p0, p2, p4);
			VrbTriangle t9 = new VrbTriangle(p2, p4, p5);
			VrbTriangle t10 = new VrbTriangle(p2, p3, p7);
			VrbTriangle t11 = new VrbTriangle(p2, p4, p7);

			VrbFace f0 = new VrbFace(t0, t1);
			VrbFace f1 = new VrbFace(t2, t3);
			VrbFace f2 = new VrbFace(t4, t5);
			VrbFace f3 = new VrbFace(t6, t7);
			VrbFace f4 = new VrbFace(t8, t9);
			VrbFace f5 = new VrbFace(t10, t11);

			List<VrbFace> fList = new List<VrbFace>();
			fList.Add(f0);
			fList.Add(f1);
			fList.Add(f2);
			fList.Add(f3);
			fList.Add(f4);
			fList.Add(f5);
			new VrbObject(fList);
		}

		public static void movePoint(VrbPoint p, float dx, float dy, float dz)
		{
			p.x += dx;
			p.y += dy;
			p.z += dz;
		}

		public static void moveEdge(int pIndex, float dx, float dy, float dz)
		{

		}

		public static void moveFace(VrbFace f, float dx, float dy, float dz)
		{
			foreach (VrbPoint p in f.getFacePoints())
			{
				movePoint(p, dx, dy, dz);
			}
		}

		public static bool readObj(string path)
		{
			return true;
		}

		public static bool saveObj(string path)
		{
			return true;
		}
		
	}
}
