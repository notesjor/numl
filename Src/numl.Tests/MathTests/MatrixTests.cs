﻿using System;
using System.IO;
using Xunit;
using numl.Math.LinearAlgebra;
using numl.Utils;
using System.Diagnostics;
using numl.Tests.SerializationTests;

namespace numl.Tests.MathTests
{
    /// <summary>
    /// Summary description for MatrixTests
    /// </summary>
    [Trait("Category", "Math")]
    public class MatrixTests
    {
        private Matrix _test = new[,]
            {{ 1, 2, 3, 4},
             { 4, 5, 6, 7},
             { 7, 8, 9, 10}};

        [Fact]
        public void Test_Matrix_Vector_Enumeration_Row()
        {
            Vector[] a = new Vector[]
            {
                new[] { 1, 2, 3, 4 },
                new[] { 4, 5, 6, 7 },
                new[] { 7, 8, 9, 10},
            };

            for (int i = 0; i < 3; i++)
                Assert.True(a[i] == _test[i, VectorType.Row]);
        }

        [Fact]
        public void Test_Matrix_Vector_Enumeration_Col()
        {
            Vector[] a = new Vector[]
            {
                new[] { 1, 4, 7 },
                new[] { 2, 5, 8 },
                new[] { 3, 6, 9 },
                new[] { 4, 7, 10}
            };

            for (int i = 0; i < 4; i++)
                Assert.True(a[i] == _test[i, VectorType.Col]);
        }

        [Fact]
        public void Test_Matrix_Vector_Enumeration_Row_Transpose()
        {
            Vector[] a = new Vector[]
            {
                new[] { 1, 4, 7 },
                new[] { 2, 5, 8 },
                new[] { 3, 6, 9 },
                new[] { 4, 7, 10}
            };

            for (int i = 0; i < 4; i++)
                Assert.True(a[i] == _test.T[i, VectorType.Row]);
        }

        [Fact]
        public void Test_Matrix_Vector_Enumeration_Col_Transpose()
        {
            Vector[] a = new Vector[]
            {
                new[] { 1, 2, 3, 4 },
                new[] { 4, 5, 6, 7 },
                new[] { 7, 8, 9, 10 },
            };

            for (int i = 0; i < 3; i++)
                Assert.Equal(a[i], _test.T[i, VectorType.Col]);
        }

        [Fact]
        public void Matrix_Serialize_Test()
        {
            string path = Path.Combine(BaseSerialization.GetPath(GetType()), "matrix_serialize_test.json");

            Matrix m1 = new[,] {
                { System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2, System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2 },
                { System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2, System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2 },
                { System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2, System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2 },
                { System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2, System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2 },
                { System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2, System.Math.PI, System.Math.PI / 2.3, System.Math.PI * 1.2 }
            };

            // serialize
            // ensure we delete the file first or we may have extra data
            if (File.Exists(path)) File.Delete(path);
            m1.Save(path);
            
            // deserialize
            Matrix m2 = Matrix.Load(path);
            Assert.Equal(m1, m2);
        }

        [Fact]
        public void Matrix_Equal_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 7},
                 {7, 8, 9, 10}};

            Matrix two = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 7},
                 {7, 8, 9, 10}};

            Assert.Equal(true, one.Equals(two));
            Assert.Equal(true, one == two);
        }

        [Fact]
        public void Matrix_Not_Equal_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 7},
                 {7, 8, 10, 9}};

            Matrix two = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 7},
                 {7, 8, 9, 10}};

            Assert.Equal(false, one.Equals(two));
            Assert.Equal(false, one == two);
            Assert.Equal(true, one != two);
        }

        [Fact]
        public void Matrix_Identity_Test()
        {
            Matrix eye1 = new[,]
                {{1, 0, 0},
                 {0, 1, 0},
                 {0, 0, 1}};

            Matrix eye2 = new[,]
                {{1, 0, 0, 0},
                 {0, 1, 0, 0},
                 {0, 0, 1, 0}};

            Matrix eye3 = new[,]
                {{1, 0, 0},
                 {0, 1, 0},
                 {0, 0, 1},
                 {0, 0, 0}};

            Assert.Equal(eye1, Matrix.Identity(3));
            Assert.Equal(eye2, Matrix.Identity(3, 4));
            Assert.Equal(eye3, Matrix.Identity(4, 3));
        }

        [Fact]
        public void Matrix_Trace_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 10}};

            Matrix two = new[,]
                {{1, 2, 3, 9},
                 {4, 5, 6, 12},
                 {7, 8, 10, 13}};

            Matrix three = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 10},
                 {11,12,13}};

            Assert.Equal(16, one.Trace());
            Assert.Equal(16, two.Trace());
            Assert.Equal(16, three.Trace());
        }

        [Fact]
        public void Matrix_Zeros_Test()
        {
            Matrix one = new[,]
                {{0, 0, 0},
                 {0, 0, 0},
                 {0, 0, 0}};

            Matrix two = new[,]
                {{0, 0, 0, 0},
                 {0, 0, 0, 0},
                 {0, 0, 0, 0}};

            Matrix three = new[,]
                {{0, 0, 0},
                 {0, 0, 0},
                 {0, 0, 0},
                 {0, 0, 0}};

            Assert.Equal(one, Matrix.Zeros(3));
            Assert.Equal(two, Matrix.Zeros(3, 4));
            Assert.Equal(three, Matrix.Zeros(4, 3));
        }

        [Fact]
        public void Matrix_Transpose_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix oneT = new[,]
                {{1, 4, 7},
                 {2, 5, 8},
                 {3, 6, 9}};

            Matrix two = new[,]
                {{1, 2, 3, 9},
                 {4, 5, 6, 12},
                 {7, 8, 10, 13}};

            Matrix twoT = new[,]
                {{1, 4, 7},
                 {2, 5, 8},
                 {3, 6, 10},
                 {9,12, 13}};

            Assert.Equal(oneT, one.Transpose());
            Assert.Equal(oneT, one.T);
            Assert.Equal(one, oneT.Transpose());
            Assert.Equal(one, oneT.T);
            Assert.Equal(twoT, two.Transpose());
            Assert.Equal(twoT, two.T);
            Assert.Equal(two, twoT.Transpose());
            Assert.Equal(two, twoT.T);
        }

        [Fact]
        public void Matrix_Assign_Value_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            one[1, 1] = 14.5;
            Assert.Equal(14.5, one[1, 1]);
        }

        public void Matrix_Assign_Value_Transpose_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Assert.Throws<InvalidOperationException>(() => one[1, 1] = 14.5);
        }

        [Fact]
        public void Matrix_Assign_Value_Bad_Index_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Assert.Throws<IndexOutOfRangeException>(() => one[5, 5] = 14.5);
        }

        [Fact]
        public void Matrix_Read_Value_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 7},
                 {7, 8, 9, 10}};

            Assert.Equal(8, one[2, 1]);
        }

        [Fact]
        public void Matrix_Read_Value_Transpose_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 7},
                 {7, 8, 9, 10}};

            Assert.Equal(one[1, 2], one.T[2, 1]);
            Assert.Equal(6, one.T[2, 1]);
        }

        [Fact]
        public void Matrix_Read_Value_Bad_Index_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Assert.Throws<IndexOutOfRangeException>(() => { var d = one[5, 5]; });
        }

        [Fact]
        public void Matrix_Add_Aligned_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix two = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix sum = new[,]
                {{2, 4, 6},
                 {8, 10, 12},
                 {14, 16, 18}};

            Matrix a = new[,]
                {{1, 2, 3, 1},
                 {4, 5, 6, 2},
                 {7, 8, 9, 3}};

            Matrix b = new[,]
                {{1, 2, 3, 5},
                 {4, 5, 6, 6},
                 {7, 8, 9, 7}};

            Matrix c = new[,]
                {{2, 4, 6, 6},
                 {8, 10, 12, 8},
                 {14, 16, 18, 10}};

            Matrix d = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9},
                 {1, 2, 3}};

            Matrix e = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9},
                 {3, 2, 1}};

            Matrix f = new[,]
                {{2, 4, 6},
                 {8, 10, 12},
                 {14, 16, 18},
                 {4, 4, 4}};

            Assert.Equal(sum, one + two);
            Assert.Equal(c, a + b);
            Assert.Equal(f, d + e);
        }

        [Fact]
        public void Matrix_Add_Non_Aligned_Test_1()
        {
            Matrix a = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix b = new[,]
                {{2, 4, 6, 6},
                 {8, 10, 12, 8},
                 {14, 16, 18, 10}};

            Assert.Throws<InvalidOperationException>(
                () => { var x = a + b; });
        }

        [Fact]
        public void Matrix_Add_Non_Aligned_Test_2()
        {
            Matrix a = new[,]
                {{2, 4, 6},
                 {8, 10, 12},
                 {14, 16, 18},
                 {4, 4, 4}};

            Matrix b = new[,]
                {{2, 4, 6, 6},
                 {8, 10, 12, 8},
                 {14, 16, 18, 10}};

            Assert.Throws<InvalidOperationException>(
                () => { var x = a + b; });
        }

        [Fact]
        public void Matrix_Subtract_Aligned_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix two = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix sum = new[,]
                {{0, 0, 0},
                 {0, 0, 0},
                 {0, 0, 0}};

            Matrix a = new[,]
                {{1, 2, 3, 1},
                 {4, 5, 6, 2},
                 {7, 8, 9, 3}};

            Matrix b = new[,]
                {{1, 2, 3, 5},
                 {4, 5, 6, 6},
                 {7, 8, 9, 7}};

            Matrix c = new[,]
                {{0, 0, 0, -4},
                 {0, 0, 0, -4},
                 {0, 0, 0, -4}};

            Matrix d = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9},
                 {1, 2, 3}};

            Matrix e = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9},
                 {3, 2, 1}};

            Matrix f = new[,]
                {{0, 0, 0},
                 {0, 0, 0},
                 {0, 0, 0},
                 {-2, 0, 2}};

            Assert.Equal(sum, one - two);
            Assert.Equal(c, a - b);
            Assert.Equal(f, d - e);
        }

        [Fact]
        public void Matrix_Subtract_Non_Aligned_Test_1()
        {
            Matrix a = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix b = new[,]
                {{2, 4, 6, 6},
                 {8, 10, 12, 8},
                 {14, 16, 18, 10}};

            Assert.Throws<InvalidOperationException>(
                () => { var x = a - b; });
        }

        [Fact]
        public void Matrix_Subtract_Non_Aligned_Test_2()
        {
            Matrix a = new[,]
                {{2, 4, 6},
                 {8, 10, 12},
                 {14, 16, 18},
                 {4, 4, 4}};

            Matrix b = new[,]
                {{2, 4, 6, 6},
                 {8, 10, 12, 8},
                 {14, 16, 18, 10}};

            Assert.Throws<InvalidOperationException>(
                () => { var x = a - b; });
        }

        [Fact]
        public void Matrix_Multiply_Vector_Aligned_Test()
        {
            Matrix one = new[,]
                {{ 2,  4,  6},
                 { 8, 10, 12},
                 {14, 16, 18}};
            Vector v = new[] { 0.5, 0.5, 0.5 };

            Matrix s1 = (new Vector(new double[] { 6, 15, 24 }))
                            .ToMatrix(VectorType.Col);
            Matrix s2 = (new Vector(new double[] { 12, 15, 18 }))
                            .ToMatrix(VectorType.Row);

            Assert.Equal(s1, one * v);
            Assert.Equal(s2, v * one);
        }

        [Fact]
        public void Matrix_Sum_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3, 4},
                 {4, 5, 6, 5},
                 {7, 8, 9, 6}};

            Vector row = new[] { 12, 15, 18, 15 };
            Vector col = new[] { 10, 20, 30 };

            Assert.Equal(row, one.Sum(VectorType.Row));
            Assert.Equal(col, one.Sum(VectorType.Col));
            Assert.Equal(60, one.Sum());
        }

        [Fact]
        public void Matrix_Multiply_Aligned_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix sol = new[,]
                {{30, 36, 42},
                 {66, 81, 96},
                 {102, 126, 150}};

            Matrix a = new[,]
                {{2, 4, 6},
                 {8, 10, 12},
                 {14, 16, 18},
                 {4, 4, 4}};

            Matrix b = new[,]
                {{2, 4, 6, 6},
                 {8, 10, 12, 8},
                 {14, 16, 18, 10}};

            Matrix c = new[,]
                {{120, 144, 168, 104},
                 {264, 324, 384, 248},
                 {408, 504, 600, 392},
                 { 96, 120, 144,  96}};


            Assert.Equal(sol, one * one);
            Assert.Equal(c, a * b);
        }

        [Fact]
        public void Matrix_Multiply_Scalar_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix sol = new[,]
                {{ 2,  4,  6},
                 { 8, 10, 12},
                 {14, 16, 18}};

            Assert.Equal(sol, 2 * one);
            Assert.Equal(sol, one * 2);
        }

        [Fact]
        public void Matrix_Sub_Matrix_Test()
        {
            Matrix one = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix sol1 = new[,]
                {{ 2, 3 },
                 { 5, 6 },
                 { 8, 9 }};

            Matrix sol2 = new[,]
                {{ 5, 6 },
                 { 8, 9 }};

            Assert.Equal(sol1, one.GetMatrix(1, 2, 0, 2));
            Assert.Equal(sol2, one.GetMatrix(1, 2, 1, 2));
        }

        [Fact]
        public void Matrix_Multiply_Non_Aligned_Test()
        {
            Matrix a = new[,]
                {{1, 2, 3},
                 {4, 5, 6},
                 {7, 8, 9}};

            Matrix b = new[,]
                {{2, 4, 6},
                 {8, 10, 12},
                 {14, 16, 18},
                 {4, 4, 4}};

            Assert.Throws<InvalidOperationException>(
                () => { var x = a * b; });
        }

        [Fact]
        public void Matrix_Inverse_Test()
        {
            Matrix a = new[,]
                {{4, 3},
                 {3, 2}};

            Matrix aInv = new[,]
                {{-2,  3},
                 { 3, -4}};

            Matrix b = new[,]
                {{ 1,  2,  3},
                 { 0,  4,  5},
                 { 1,  0,  6}};

            Matrix bInv = new Matrix(new double[,]
                {{ 12.0/11,  -6.0/11,  -1.0/11},
                 {  5.0/22,   3.0/22,  -5.0/22},
                 { -2.0/11,   1.0/11,   2.0/11}});

            Assert.Equal(aInv, a ^ -1);
            Assert.Equal(bInv.ToString(), (b ^ -1).ToString());
        }


        [Fact]
        public void Matrix_Cholesky_Test()
        {
            Matrix m = new[,]
                {{ 25, 15, -5},
                 { 15, 18,  0},
                 { -5,  0, 11}};

            Matrix sol = new[,]
                {{  5, 0, 0},
                 {  3, 3, 0},
                 { -1, 1, 3}};

            Assert.Equal(sol, Matrix.Cholesky(m));
            Assert.Equal(sol, m.Cholesky());
        }

        [Fact]
        public void Matrix_LLS_Test()
        {
            Matrix A = new[,]
                {{ 3, -6},
                 { 4, -8},
                 { 0,  1}};

            Vector b = new[] { -1, 7, 2 };
            Vector sol = new[] { 5, 2 };

            // LLS implementation
            Vector x = A / b;
            Assert.Equal(sol, x);

            // this should fire regular inverse
            // need to be a bit smarter about using straight
            // inverse here...
            // Cholesky if positive diagonal or
            // LU
            Vector y = (A.T * A) / (A.T * b).ToVector();
            Assert.Equal(sol, y);
        }

        [Fact]
        public void Matrix_QR_Test()
        {
            Matrix A = new[,] {{ 4,  1,  2},
                               { 1,  4,  0},
                               { 2,  0,  4}};

            var t = Matrix.QR(A);
            var Q = t.Item1;
            var R = t.Item2;

            // close enough...
            var diff = A.Norm() - (Q * R).Norm();
            Assert.Equal(0, diff);
        }

        [Fact]
        public void Matrix_Doolittle_Pivot_Test()
        {
            Matrix A = new[,] {{ 1, 4, 2, 3 },
                               { 1, 2, 1, 0 },
                               { 2, 6, 3, 1 },
                               { 0, 0, 1, 4 }};

            Matrix B = new[,] {{ 2,  3,  1,  2 },
                                {-1,  2,  7,  5 },
                                {-4, -3,  4,  2 },
                                { 3,  1,  6,  3 }};

            Matrix C = new[,] {{ 1, 4, 2, 3 },
                               { 1, 2, 1, 0 },
                               { 2, 6, 3, 1 },
                               { 0, 0, 1, 4 }};

            Matrix P = new[,] {{ 0, 0, 0, 1 },
                               { 0, 1, 0, 0 },
                               { 1, 0, 0, 0 },
                               { 0, 0, 1, 0 }};


            var I = Matrix.Pivot(B);
            //Assert.Equal(P, I);
        }

        [Fact]
        public void Matrix_LU_Test()
        {
            Matrix A = new[,] {{ 1, 2, 0 },
                               { 3, 6, -1},
                               { 1, 2, 1 }};

            Matrix B = new[,] {{ 7,  3, -1,  2},
                               { 3,  8,  1, -4},
                               {-1,  1,  4, -1},
                               { 2, -4, -1,  6}};

            Matrix C = new[,] {{ 1, 4, 2, 3 },
                               { 1, 2, 1, 0 },
                               { 2, 6, 3, 1 },
                               { 0, 0, 1, 4 }};

            Matrix D = new[,] {{ 2,  3,  1,  2 },
                               {-1,  2,  7,  5 },
                               {-4, -3,  4,  2 },
                               { 3,  1,  6,  3 }};

            var t = Matrix.LU(D);
            var P = t.Item1;
            var L = t.Item2;
            var U = t.Item3;

            var T = Matrix.Stack(P, L).Stack(U);
        }

        [Fact]
        public void Matrix_Extract_Test()
        {
            Matrix A = new[,]
                {{  1,  2,  4 },
                 {  1,  6,  2 },
                 {  3,  1,  1 }};


            Matrix sol = new[,]
                {{ 6, 2},
                 { 1, 1}};

            Assert.Equal(sol, A.Extract(1, 1, 2, 2));

            Matrix B = new[,]
                {{  1,  2,  4,  9 },
                 {  1,  6,  2,  7 },
                 {  3,  1,  1,  4 },
                 {  4,  2,  3,  2 }};

            Matrix bSol = new[,]
                {{  2,  7 },
                 {  1,  4 },
                 {  3,  2 }};

            Assert.Equal(bSol, B.Extract(2, 1, 2, 3));
        }

        [Fact]
        public void Matrix_Covariance_Test()
        {
            // from: http://www.itl.nist.gov/div898/handbook/pmc/section5/pmc541.htm

            Matrix x = new[,]
               {{ 4.0, 2.0, .60 },
                { 4.2, 2.1, .59 },
                { 3.9, 2.0, .58 },
                { 4.3, 2.1, .62 },
                { 4.1, 2.2, .63 }};

            Matrix covTruth = new[,]
               {{ 0.02500, 0.00750, 0.00175 },
                { 0.00750, 0.00700, 0.00135 },
                { 0.00175, 0.00135, 0.00043 }};

            var cov = x.Covariance().Round(5);
            Assert.Equal(covTruth, cov);
        }

        [Fact]
        public void Matrix_Determinant_Test()
        {

            Matrix x = new[,]
               {{ -2, 2, 3 },
                { -1, 1, 3 },
                { 2, 0, 1 }};

            Assert.Equal(6, x.Det());

            Matrix m = new[,]
                {{ 1, 2,  2, 1 },
                 { 1, 2,  4, 2 },
                 { 2, 7,  5, 2 },
                 {-1, 4, -6, 3 }};

            // -42 + residual
            var det = m.Det();

            Matrix q = new[,]
                 {{ 3,   2,  -1,   4, },
                  { 2,   1,   5,   7, },
                  { 0,   5,   2,  -6, },
                  { -1,   2,   1,   0, }};

            // -418
            var qd = q.Det();
        }

        [Fact]
        public void Matrix_Reshape_Rows_Test()
        {
            Vector v = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Matrix x = v.Reshape(5, VectorType.Row);

            Assert.Equal(v[7], x[2, 1]);
            Assert.Equal(v[5], x[0, 1]);
            Assert.Equal(v[4], x[4, 0]);
            Assert.Equal(v[9], x[4, 1]);
        }

        [Fact]
        public void Matrix_Reshape_Cols_Test()
        {
            Vector v = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Matrix y = v.Reshape(5, VectorType.Col, VectorType.Col);

            Assert.Equal(v[2], y[0, 2]);
            Assert.Equal(v[1], y[0, 1]);
            Assert.Equal(v[5], y[1, 0]);
            Assert.Equal(v[9], y[1, 4]);
        }

        [Fact]
        public void Matrix_Unshape_Test()
        {
            Vector v = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Matrix x = v.Reshape(5, VectorType.Row);
            Matrix y = v.Reshape(5, VectorType.Col);

            Vector vx = x.Unshape();
            Assert.Equal(v[1], vx[1]);
            Assert.Equal(v[2], vx[2]);
            Assert.Equal(v[8], vx[8]);
            Assert.Equal(v[9], vx[9]);

            Vector vy = y.Unshape();
            Assert.Equal(v[1], vy[1]);
            Assert.Equal(v[2], vy[2]);
            Assert.Equal(v[8], vy[8]);
            Assert.Equal(v[9], vy[9]);
        }

        [Fact]
        public void Matrix_Sort_Rows_Test()
        {
            Matrix m1 = new[,]
            {
                { 1, 3, 6, 5 }, // 0
                { 3, 6, 4, 4 }, // 2
                { 4, 7, 5, 3 }, // 3
                { 2, 9, 3, 2 }, // 1
                { 4, 8, 8, 8 }, // 4
            };

            Vector v = new Vector(new double[] { 0, 3, 1, 2, 4 });

            Vector indices;

            Matrix m1s = m1.Sort(k => k[0], VectorType.Row, true, out indices);

            Assert.Equal(m1[0, 0], m1s[0, 0]);
            Assert.Equal(m1[3, 0], m1s[1, 0]);
            Assert.Equal(m1[1, 0], m1s[2, 0]);
            Assert.Equal(m1[2, 0], m1s[3, 0]);
            Assert.Equal(m1[4, 0], m1s[4, 0]);

            Assert.Equal(m1[0, 1], m1s[0, 1]);
            Assert.Equal(m1[3, 1], m1s[1, 1]);
            Assert.Equal(m1[1, 1], m1s[2, 1]);
            Assert.Equal(m1[2, 1], m1s[3, 1]);
            Assert.Equal(m1[4, 1], m1s[4, 1]);

            Assert.Equal(v[0], indices[0]);
            Assert.Equal(v[1], indices[1]);
            Assert.Equal(v[2], indices[2]);
            Assert.Equal(v[3], indices[3]);
            Assert.Equal(v[4], indices[4]);
        }

        [Fact]
        public void Matrix_Sort_Columns_Test()
        {

            Vector v = new Vector(new double[] { 1, 3, 2, 0 });

            Matrix m1 = new[,]
            {
               // 3  0  2  1
                { 4, 1, 3, 2 },
                { 1, 2, 3, 4 },
                { 7, 9, 8, 6 },
                { 6, 9, 3, 2 },
                { 6, 8, 8, 8 },
            };

            Vector indices;
            Matrix m1s = m1.Sort(k => k[0], VectorType.Col, true, out indices);

            Assert.Equal(m1[0, 3], m1s[0, 1]);
            Assert.Equal(m1[0, 0], m1s[0, 3]);
            Assert.Equal(m1[0, 2], m1s[0, 2]);
            Assert.Equal(m1[0, 1], m1s[0, 0]);

            Assert.Equal(m1[1, 3], m1s[1, 1]);
            Assert.Equal(m1[1, 0], m1s[1, 3]);
            Assert.Equal(m1[1, 2], m1s[1, 2]);
            Assert.Equal(m1[1, 1], m1s[1, 0]);

            Assert.Equal(v[0], indices[0]);
            Assert.Equal(v[1], indices[1]);
            Assert.Equal(v[2], indices[2]);
            Assert.Equal(v[3], indices[3]);
        }

        [Theory]
        [InlineData(0, false, VectorType.Row, false)]
        [InlineData(1, false, VectorType.Row, false)]
        [InlineData(2, false, VectorType.Row, false)]
        [InlineData(0, true, VectorType.Row, false)]
        [InlineData(1, true, VectorType.Row, false)]
        [InlineData(2, true, VectorType.Row, false)]
        [InlineData(0, false, VectorType.Col, false)]
        [InlineData(1, false, VectorType.Col, false)]
        [InlineData(2, false, VectorType.Col, false)]
        [InlineData(3, false, VectorType.Col, false)]
        [InlineData(0, true, VectorType.Col, false)]
        [InlineData(1, true, VectorType.Col, false)]
        [InlineData(2, true, VectorType.Col, false)]
        [InlineData(3, true, VectorType.Col, false)]
        [InlineData(0, false, VectorType.Row, true)]
        [InlineData(1, false, VectorType.Row, true)]
        [InlineData(2, false, VectorType.Row, true)]
        [InlineData(3, false, VectorType.Row, true)]
        [InlineData(0, true, VectorType.Row, true)]
        [InlineData(1, true, VectorType.Row, true)]
        [InlineData(2, true, VectorType.Row, true)]
        [InlineData(3, true, VectorType.Row, true)]
        [InlineData(0, false, VectorType.Col, true)]
        [InlineData(1, false, VectorType.Col, true)]
        [InlineData(2, false, VectorType.Col, true)]
        [InlineData(0, true, VectorType.Col, true)]
        [InlineData(1, true, VectorType.Col, true)]
        [InlineData(2, true, VectorType.Col, true)]
        public void Matrix_Insert_Test(int index, bool insertAfter, VectorType vectorType, bool isTransposed)
        {
            Vector v = (vectorType == VectorType.Row) ^ isTransposed ?
                            new double[] { 1, 3, 2, 0 } :
                            new double[] { 2, 1, 0 };

            Matrix A = new[,]
            {
                { 4, 1, 3, 2 },
                { 1, 2, 3, 4 },
                { 7, 9, 8, 6 }
            };

            if (isTransposed)
                A = A.T;

            var rows = A.Rows;
            var columns = A.Cols;

            var B = A.Insert(v, index, vectorType, insertAfter);

            Assert.Equal(A.Rows, rows);
            Assert.Equal(A.Cols, columns);

            if (vectorType == VectorType.Row)
            {
                Assert.Equal(B.Rows, rows + 1);
                Assert.Equal(B.Cols, columns);
            }
            else
            {
                Assert.Equal(B.Rows, rows);
                Assert.Equal(B.Cols, columns + 1);
            }

            var dimension = vectorType == VectorType.Row ? rows : columns;

            for (var i = 0; i < dimension + 1; i++)
            {
                if (index == dimension - 1 && insertAfter)
                    Assert.Equal(v, B[dimension, vectorType]);
                else if (i == index)
                    Assert.Equal(v, B[i, vectorType]);
                else if(i < index)
                    Assert.Equal(A[i, vectorType], B[i, vectorType]);
                else
                    Assert.Equal(A[i - 1, vectorType], B[i, vectorType]);
            }
        }
    }
}
