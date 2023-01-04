using System;
using System.Drawing;
using System.Collections;
using System.Numerics;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using static TSP_CSharp.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Reflection.Emit;

/*
 Travelling Salesman Problem
The Travelling Salesman Problem (TSP) is the challenge of finding the shortest yet most efficient route for a person to take given a list of specific destinations.
It is a well-known algorithmic problem in the fields of computer science and operations research.
There are obviously a lot of different routes to choose from but finding the best one (the one that will require the least distance or cost) 
is what mathematicians and computer scientists have spent decades trying to solve for.
 */

namespace TSP_CSharp
{
    public partial class Form1 : Form
    {
        // Declerations
        Graphics g;
        Pen p;
        Point cursor;
        List<Point> cities = new List<Point>();
        List<double> distances= new List<double>();
        List<Point> newCityOrder = new List<Point>();
        double Tempreture;
        double coolingRate;
        int maxIter=0;
        double currentDistance = 0;
        double nextDistance = 0;
        double shortestdistance = 0;
        double deltaDistance = 0;
        //Dictionary to store the combination of distances as a primary key with the order of cities associated with
        Dictionary<double , List<Point>> bestRouteDic = new Dictionary<double, List<Point>>();
        public static List<Point> ShuffleCities(List<Point> C)
        {
            
            Random random = new Random();
            List<Point> newOrder = new List<Point>();
            for (int i = 0; i < C.Count; i++)
            {
                newOrder.Add(C[i]);

            }
            int firstRandomCityIndex = random.Next(1, newOrder.Count);
            int secondRandomCityIndex = random.Next(1, newOrder.Count);

            Point dummy = newOrder[firstRandomCityIndex];
            newOrder[firstRandomCityIndex] = newOrder[secondRandomCityIndex];
            newOrder[secondRandomCityIndex] = dummy;

            return newOrder;
        }
        public static double getTotalDistance(List<Point> C)
        {
            double distance = 0;
            for(int i=0; i<C.Count-1; i++)
            {
                distance+= Math.Sqrt((Math.Pow(C[i].X - C[i + 1].X, 2) +
                    Math.Pow(C[i].Y - C[i + 1].Y, 2)));
            }

            return distance;
        }
        public Form1()
        {
            InitializeComponent();

            //Inizalization
            g = panel1.CreateGraphics();
            p = new Pen(Color.Black, 5);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtNumCity.Text = "0";
            txtInitialTemp.Text = "10000";
            trackBar1.Value = 0;
            label3.Text = "Cooling Rate:   "+trackBar1.Value;
            TxtMaxIter.Clear();
            TxtMaxIter.Text= "5000";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Default values
            txtNumCity.Text = "0";
            txtInitialTemp.Text = "10000";
            TxtMaxIter.Text = "5000";
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

            if (drawcheck.Checked == true)
            {
                g.DrawEllipse(p, cursor.X - 5, cursor.Y - 5, 5, 5);
                cities.Add(new Point(cursor.X, cursor.Y));

                listBox1.Items.Add("X: " + cursor.X + "   Y:" + cursor.Y);

                txtNumCity.Text = cities.Count.ToString();
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            cursor = panel1.PointToClient(Cursor.Position);

            mousestatus.Text = "X: " + cursor.X + " Y: " + cursor.Y;
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            if (int.Parse(txtNumCity.Text) < 10)
            {
                MessageBox.Show("Please add at least 10 cities!");
            }
            else
            {
                panel1.CreateGraphics().DrawLines(Pens.Red, cities.ToArray());
                panel1.CreateGraphics().DrawLine(Pens.Red, cities.ElementAt(0), cities.ElementAt(cities.Count-1));

            // Find the initial path and print path distance
            for (int i = 0; i < cities.Count; i++)
            {
                currentDistance= getTotalDistance(cities);
                    shortestdistance = currentDistance;

            }
            InPath.Text = shortestdistance + "  KM";

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            maxIter = int.Parse(TxtMaxIter.Text);

            Tempreture = double.Parse(txtInitialTemp.Text);
            coolingRate = trackBar1.Value;
            double CR = coolingRate / 1000;

            distances.Clear();
            listBox2.Items.Clear();

            for (int i = 0; i < maxIter; i++)
            {
                if (Tempreture <= 2)
                {
                    
                    ShortPath.Text = getTotalDistance(newCityOrder).ToString() + " KM";
                }
                else
                {

                    newCityOrder = ShuffleCities(cities);
                    nextDistance =getTotalDistance(newCityOrder);

                    // Storing the set of points in to the dictionary with the unique distances
                    if(!bestRouteDic.ContainsKey(nextDistance))
                    {
                        bestRouteDic.Add(nextDistance, newCityOrder);

                    }

                    deltaDistance = nextDistance - currentDistance;
                    if (deltaDistance < 0)
                    {
                        currentDistance = nextDistance;

                        if (shortestdistance > currentDistance)
                        {
                            shortestdistance = currentDistance;

                        }
                    }
                    else
                    {
                        Random rand1 = new Random();
                        double rand2 = (rand1.Next(0, 1));
                        double exp = Math.Pow(2.71, -(nextDistance - currentDistance) / Tempreture);
                        if (exp > rand2)
                        {
                            currentDistance = nextDistance;

                        }

                    }
                    

                    //Cooling Tempreture
                    Tempreture *= CR;

                    // Get the miniume distance from the dictionary
                    double bestdist = bestRouteDic.Keys.Min();
                    List<Point> BCities = bestRouteDic[bestdist];

                    // Store Distances in distance list--> print distance combinations to listbox
                    distances.Add(nextDistance);
                    listBox2.Items.Add(distances[i]);

                    // Draw the best path according to shortest distance
                    panel1.CreateGraphics().Clear(Color.White);
                    for (int n = 0; n < BCities.Count; n++)
                    {

                        g.DrawEllipse(p, BCities[n].X - 5, BCities[n].Y - 5, 5, 5);

                        // Connect Cities with paths
                        g.DrawLines(Pens.Blue, BCities.ToArray());
                        g.DrawLine(Pens.Blue, BCities.ElementAt(0), BCities.ElementAt(BCities.Count - 1));

                        

                    }

                }

            }

            shortestdistance = distances.Min();
            ShortPath.Text = shortestdistance.ToString() + " KM";

        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            coolingRate = trackBar1.Value;
            label3.Text = "Cooling Rate:   "+(coolingRate / 1000).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            for (int i = 0; i < newCityOrder.Count; i++)
            {
                listBox2.Items.Add(newCityOrder[i].ToString());

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void drawcheck_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

}
