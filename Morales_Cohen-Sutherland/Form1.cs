using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Morales_Cohen_Sutherland
{
    public partial class Form1 : Form
    {
        private Bitmap bmp;
        private Graphics g;
        private CPoint firstPoint = null;
        private CPoint secondPoint = null;
        private List<Tuple<CPoint, CPoint>> originalLines = new List<Tuple<CPoint, CPoint>>();
        private CPoint tempFirstPoint = null;
        private Rectangle clipWindow;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(picCanvas.Width, picCanvas.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // Tamaño del recorte (ajústalo si deseas)
            int clipWidth = 200;
            int clipHeight = 150;
            int centerX = (picCanvas.Width - clipWidth) / 2;
            int centerY = (picCanvas.Height - clipHeight) / 2;
            clipWindow = new Rectangle(centerX, centerY, clipWidth, clipHeight);

            g.DrawRectangle(Pens.Black, clipWindow);
            picCanvas.Image = bmp;

            // Configurar columnas del DataGridView (si no está en diseñador)
            dgvLines.Columns.Add("X1", "X1");
            dgvLines.Columns.Add("Y1", "Y1");
            dgvLines.Columns.Add("X2", "X2");
            dgvLines.Columns.Add("Y2", "Y2");
            dgvLines.Columns.Add("Estado", "Estado");
        }

        private void picCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (tempFirstPoint == null)
            {
                tempFirstPoint = new CPoint(e.X, e.Y);
            }
            else
            {
                var secondPoint = new CPoint(e.X, e.Y);
                originalLines.Add(Tuple.Create(tempFirstPoint, secondPoint));

                // Dibujar línea original
                g.DrawLine(Pens.Gray, tempFirstPoint.X, tempFirstPoint.Y, secondPoint.X, secondPoint.Y);
                picCanvas.Image = bmp;

                tempFirstPoint = null;
            }
        }

        private void btnTrim_Click(object sender, EventArgs e)
        {
            dgvLines.Rows.Clear();
            var clipper = new CCohenSutherland(clipWindow);

            foreach (var line in originalLines)
            {
                // Copiar puntos originales
                CPoint originalP1 = line.Item1;
                CPoint originalP2 = line.Item2;

                // Clonar para recorte
                CPoint clippedP1 = new CPoint(originalP1.X, originalP1.Y);
                CPoint clippedP2 = new CPoint(originalP2.X, originalP2.Y);

                bool isVisible = clipper.ClipLine(ref clippedP1, ref clippedP2);

                using (Pen fadedPen = new Pen(Color.FromArgb(100, Color.Gray)))
                {
                    // Si la línea fue recortada, dibujar solo los segmentos exteriores en gris
                    if (isVisible)
                    {
                        // Segmento antes del recorte
                        if (originalP1.X != clippedP1.X || originalP1.Y != clippedP1.Y)
                            g.DrawLine(fadedPen, originalP1.X, originalP1.Y, clippedP1.X, clippedP1.Y);

                        // Segmento después del recorte
                        if (originalP2.X != clippedP2.X || originalP2.Y != clippedP2.Y)
                            g.DrawLine(fadedPen, clippedP2.X, clippedP2.Y, originalP2.X, originalP2.Y);

                        // Segmento visible (recortado)
                        g.DrawLine(Pens.Red, clippedP1.X, clippedP1.Y, clippedP2.X, clippedP2.Y);

                        dgvLines.Rows.Add(
                            ToCartesianX(clippedP1.X), ToCartesianY(clippedP1.Y),
                            ToCartesianX(clippedP2.X), ToCartesianY(clippedP2.Y),
                            "Visible"
                        );
                    }
                    else
                    {
                        // Si no es visible, dibujar todo el segmento opaco
                        g.DrawLine(fadedPen, originalP1.X, originalP1.Y, originalP2.X, originalP2.Y);
                        dgvLines.Rows.Add("-", "-", "-", "-", "No visible");
                    }
                }
            }

            picCanvas.Image = bmp;  
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            bmp = new Bitmap(picCanvas.Width, picCanvas.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, clipWindow);
            picCanvas.Image = bmp;

            tempFirstPoint = null;
            originalLines.Clear();
            dgvLines.Rows.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int ToCartesianX(int x)
        {
            return x - picCanvas.Width / 2;
        }

        private int ToCartesianY(int y)
        {
            return (picCanvas.Height / 2) - y;
        }

    }
}
