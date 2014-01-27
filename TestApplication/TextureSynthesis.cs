using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestApplication
{
    //http://graphics.cs.cmu.edu/people/efros/research/EfrosLeung.html
    //https://github.com/ashovlin/texturesynth
    //http://pages.cs.wisc.edu/~vavra/cs766/efros99Code/
    //https://github.com/zeeshanlakhani/compphoto_texturesynth
    public class Efros99
    {
        private readonly WriteableBitmap _input;
        private readonly int _nhsize;
        public static int DefaultNeighborhoodSize = 5;

        /** This sets up the algorithm.
         *  @param input This is the texture to sample from.
         *  @param nhsize This is the width of the square neighborhood used
         *                when choosing parts of the texture to sample from.
         */
        public Efros99(WriteableBitmap input, int nhsize)
        {

            if (nhsize < 2)
                nhsize = DefaultNeighborhoodSize;
            _nhsize = nhsize;
            if (input == null || input.PixelWidth < nhsize
                || input.PixelHeight < nhsize)
                throw new ArgumentException("Image is too small");
            _input = input;
        }
        
        /** This synthesizes a new texture image with the given dimensions.
         */
        public WriteableBitmap Synthesize(int outwidth, int outheight)
        {
            // make sure that the output is big enough that the
            //  neighborhood does not wrap around onto unsynthesized pixels
            //  where it expects synthesized ones
            if (outwidth < _nhsize || outheight < _nhsize)
                throw new ArgumentException("Output size is too small");

            // create the output image
            WriteableBitmap output = _input.Clone();
            //output.clear(0);

            Random random = new Random();

            // copy a few pixels to get started
            OutputNHood outview = new OutputNHood(output, -1, 0, _nhsize);
            int x = (int) (random.NextDouble()*(_input.PixelWidth - _nhsize/2 - 1));
            int y = (int) (random.NextDouble()*(_input.PixelHeight - _nhsize/2 - 1));
            View inview = new View(_input, x, y);
            for (x = 0; x < _nhsize/2; x++)
            {
                outview.NextPixel();
                outview.PutCenterSample(inview.GetSample(x, 0));
            }

            double[,] dists = new double[_input.PixelHeight,_input.PixelWidth];
            double[,] weights = Util.Gaussian(_nhsize);

            // loop over the rest of the image
            inview.SetCorner(0, 0);
            while (outview.NextPixel())
            {
                // get the distances for this neighborhood
                Location2D bestloc = Util.GetSSDs(dists, weights, outview, _input);
                double bestval = dists[bestloc.Row, bestloc.Column];

                // pick one of the close matches
                //double threshold = ( bestval == 0 ? .1 : bestval * 1.1 );
                //double threshold = bestval + 0.05;
                double threshold = bestval;
                List<Location2D> list = Util.LessThanEqual(dists, threshold);
                int choiceindex = (int) (random.NextDouble()*list.Count);
                Location2D choice = list[choiceindex];
                //TwoDLoc choice = bestloc;

                // set the value
                int[] sample = inview.GetSample(choice.X, choice.Y);
                outview.PutCenterSample(sample);

                //if( outview.CenterX == output.PixelWidth-1 ) {
                //    System.out.println("Done with row "+outview.getCenterY());
                //}
            }
            return output;
        }
    }

    public class Util
    {
        /** This returns a square 2D normalized Gaussian filter of the given size.
        */
        public static double[,] Gaussian(int length)
        {
            if (length%2 == 0)
                length++;

            // this stddev puts makes a good spread for a given size
            double stddev = length/4.9;

            // make a 1d gaussian kernel
            double[] oned = new double[length];
            for (int i = 0; i < length; i++)
            {
                int x = i - length/2;
                double exponent = x*x/(-2*stddev*stddev);
                oned[i] = System.Math.Exp(exponent);
            }

            // make the 2d version based on the 1d
            double[,] twod = new double[length,length];
            double sum = 0.0;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    twod[i, j] = oned[i]*oned[j];
                    sum += twod[i, j];
                }
            }

            // normalize
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    twod[i, j] /= sum;

            return twod;
        }

        /** This searches the given array for all non-negative values less than
         *  or equal to a given threshold and returns the list
         *  of array indicies of matches. Negative values are assumed
         *  to be invalid and thus are ignored.
         *  
         *  @return This returns a list of TwoDLoc objects.
         */
        public static List<Location2D> LessThanEqual(double[,] vals, double threshold)
        {
            List<Location2D> list = new List<Location2D>();
            for (int r = 0; r < vals.GetLength(0); r++)
                for (int c = 0; c < vals.GetLength(1); c++)
                    if (vals[r, c] >= 0 && vals[r, c] <= threshold)
                        list.Add(new Location2D(r, c));
            return list;
        }

        /** This compares the output neighborhood to each valid neighborhood
         *  in the given image and fills in the array with the resulting
         *  distances based on the SSDs and weights.
         *  The coordinates of the best match are returned.
         *  @param dists This is the array that the distance values will be stored
         *              in. It must be the same size as the image with
         *              x as the first dimension and y as the second. After
         *              being filled, then array element x,y will have the
         *              SSD calculated when the nhood was centered at x,y
         *              (-1 if x,y was not such a center).
         *  @param weights This array must be the same size as the neighborhood.
         *           It will be used to weight the distances such that
         *           the output is a weighted sum of the SSDs of each
         *           valid pixel in the neighborhood.
         *  @return This returns the coordinates of the best match.
         *          The first element is the x and the second is the y.
         */
        public static Location2D GetSSDs(double[,] dists, double[,] weights, OutputNHood outhood, WriteableBitmap image)
        {
            // clear the array with invalid numbers
            for (int i = 0; i < dists.GetLength(0); i++)
                for (int j = 0; j < dists.GetLength(1); j++)
                    dists[i, j] = -1;

            // set up a neighborhood in the input image
            int size = outhood.Size;
            InputNHood hood = new InputNHood(image, size);

            // do the first location
            double bestdist = Dist(outhood, hood, weights);
            int bestx = hood.CenterX;
            int besty = hood.CenterY;
            dists[besty, bestx] = bestdist;

            // loop over the input image to do the rest
            while (hood.NextPixel())
            {

                double dist = Dist(outhood, hood, weights);
                int x = hood.CenterX;
                int y = hood.CenterY;
                dists[y, x] = dist;
                if (dist < bestdist)
                {
                    bestdist = dist;
                    bestx = x;
                    besty = y;
                }
            }

            return new Location2D(besty, bestx);
        }

        /** This computes the sum (accross channels) of squared differences
         *  between the pixel values at the given coordinate in the given
         *  views.
         */
        public static int SSD(View view1, View view2, int x, int y)
        {
            int[] vals = view1.GetSample(x, y);
            int[] vals2 = view2.GetSample(x, y);

            int diff = vals[0] - vals2[0];
            int sum = diff*diff;
            diff = vals[1] - vals2[1];
            sum += diff*diff;
            diff = vals[2] - vals2[2];
            sum += diff*diff;

            return sum;
        }

        /** This returns the sum of squared differences, weighted,  between
         *  the pixels in a partially filled neighborhood and the pixels
         *  in a filled view. Only pixels straight to the left of the
         *  nhood center or on a row above the nhood center are considered.
         *  @param w This array must be the same size as the neighborhood.
         *           It will be used to weight the distances such that
         *           the output is a weighted sum of the SSDs of each
         *           valid pixel in the neighborhood.
         */
        public static double Dist(OutputNHood nhood, View view, double[,] w)
        {
            double sum = 0;

            // get some info about the nhood's boundaries
            int viewcenter = nhood.ViewCenter;
            int minviewy = nhood.GetFirstValidRow();
            int minviewx = nhood.GetFirstValidCol();
            int maxviewx = nhood.GetLastValidCol();

            // loop over the rows above this row
            for (int viewy = minviewy; viewy < viewcenter; viewy++)
                // the entire row should be valid
                for (int viewx = minviewx; viewx < maxviewx; viewx++)
                    sum += SSD(nhood, view, viewx, viewy)*w[viewy, viewx];

            // loop over the pixels in this row to the left of the center
            for (int viewx = minviewx; viewx < viewcenter; viewx++)
                sum += SSD(nhood, view, viewx, viewcenter)*w[viewcenter, viewx];

            return sum;
        }
    }

    public class Location2D
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public int X { get { return Column; } }
        public int Y { get { return Row; } }

        public Location2D(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

    public class OutputNHood : NHood
    {
        /** Calls the NHood constructor with these same params. */
        public OutputNHood(WriteableBitmap image, int cx, int cy, int size) : base(image, cx, cy, size)
        {
        }

        /** Sets the center to 0,0 and calls the NHood constructor */
        public OutputNHood(WriteableBitmap image, int size)
            : this(image, 0, 0, size)
        {
        }

        /** This shifts the neighborhood's center to the next pixel.
         *  The next pixel is one to the right of the current center
         *  unless the current center is at the right edge. If the
         *  center is at the left edge, the next pixel is the left
         *  edge and one row farther down.
         *
         *  @return This returns false if the center was already at the
         *          bottom right edge of the image and true otherwise.
         */
        public bool NextPixel()
        {
            if (CenterY + 1 == Width)
            {
                if (CenterY + 1 == Height)
                    return false;

                // wrap around to the next row
                CenterY++;
                CenterY = 0;
                SetCorner(CenterY - ViewCenter, CenterY - ViewCenter);
            }
            else
            {
                CenterY++;
                SetCorner(CenterY - ViewCenter, CenterY - ViewCenter);
            }

            return true;
        }

        /** This returns the first row (in view coordinates)
         *  that is within both the neighborhood and the image.
         */
        public int GetFirstValidRow()
        {
            return System.Math.Max(0, ViewCenter - CenterX);
        }

        /** This returns the last row (in view coordinates)
         *  that is within both the neighborhood and the image.
         */
        public int GetLastValidRow()
        {
            return System.Math.Min(Height - 1 - YOffset, Size - 1);
        }

        /** This returns the last column (in view coordinates)
         *  that is within both the neighborhood and the image.
         */
        public int GetLastValidCol()
        {
            return System.Math.Min(Width - 1 - XOffset, Size - 1);
        }


        /** This returns the first column (in view coordinates)
         *  that is within both the neighborhood and the image.
         */
        public int GetFirstValidCol()
        {
            return System.Math.Max(0, ViewCenter - CenterX);
        }
    }

    public class InputNHood : NHood
    {
        /** This calls NHood's constructor and verifies that the center
         *  places the neighborhood completely within the image.
         */
        public InputNHood(WriteableBitmap image, int cx, int cy, int size)
            : base(image, cx, cy, size)
        {
        }

        /** 
         *  This will set the neighborhood as close to the upper left corner
         *  of the image as possible.
         *
         *  @param image This is the image that the neighborhood is in.
         *  @param size This is the width and height (in pixels) of the nhood.
         */
        public InputNHood(WriteableBitmap image, int size)
            : this(image, size/2, size/2, size)
        {
        }

        /** This shifts the neighborhood's center to the next pixel.
         *  The center shifts to the right until it runs out of room.
         *  It then wraps around to the beginning of the next row.
         *  This operation keeps the entire neighborhood within
         *  the image boundaries.
         *  
         *  If such a move is not possible from
         *  the current location (ie stuck in the bottom right corner)
         *  then this returns false. This otherwise returns true.
         *
         *  @return This returns false if the center can not be moved
         *          any farther and this returns true otherwise.
         */
        public bool NextPixel()
        {

            // if can move right
            if (XOffset + Size < Width - 1)
            {
                CenterX++;
                SetCorner(CenterX - ViewCenter, CenterY - ViewCenter);
                return true;
            }

            // else if can move down
            if (YOffset + Size < Height)
            {
                // wrap around to the next row
                CenterY++;
                CenterX = 0 + ViewCenter;
                SetCorner(0, CenterY - ViewCenter);
                return true;
            }

            return false;
        }
    }

    public class NHood : View
    {
        /** This is the x image coord of the neighborhood's center. */
        public int CenterX { get; protected set; }

        /** This is the y image coord of the neighborhood's center. */
        public int CenterY { get; protected set; }

        /** This is the neighborhood size (1D) */
        public int Size { get; protected set; }

        /** This is the center in view coordinates = size/2 for both x and y */
        public int ViewCenter { get; protected set; }

        /** This sets up a square neighborhood that has
     *  the given center (in image coords) and size.
     *  If the size is even, then the center will be in the lower-right
     *  quadrant of the square.
     *
     *  @param image This is the image that the neighborhood is in.
     *  @param centerx This is the x image coordinate of the nhood's center.
     *  @param centery This is the y image coordinate of the nhood's center.
     *  @param size This is the width and height (in pixels) of the nhood.
     */
        public NHood(WriteableBitmap image, int centerx, int centery, int size) :
            base(image, centerx - size%2, centery - size%2)
        {
            CenterX = centerx;
            CenterY = centery;
            Size = size;
            ViewCenter = size/2;
        }

        /** This sets the value of the center. */
        public void PutCenterSample(int[] newvals)
        {
            PutSample(ViewCenter, ViewCenter, newvals);
        }
    }

    public class View
    {
        //protected WriteableBitmap Image;
        protected int Width;
        protected int Height;
        protected int[] Image;
        protected int XOffset, YOffset;

        public View(WriteableBitmap image, int x, int y)
        {
            Width = image.PixelWidth;
            Height = image.PixelHeight;
            Image = new int[Width * Height];

            image.CopyPixels(Image, Width * 4, 0);
            SetCorner(x, y);
        }

        public WriteableBitmap GetBitmap()
        {
            WriteableBitmap result = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);
            result.WritePixels(new Int32Rect(0, 0, Width, Height), Image, Width*4, 0);
            return result;
        }

        /** This moves the view to the specified position. */
        public void SetCorner(int x, int y)
        {
            XOffset = x;
            YOffset = y;
        }

        /** This fetches the RGB values from the given view coordinates. */
        public int[] GetSample(int x, int y)
        {
            x = ImageX(x);
            y = ImageY(y);
            int offset = x + y*Width;
            int pixel = Image[offset];
            int[] triplet = new int[3];
            triplet[0] = pixel & 0xFF;
            triplet[1] = (pixel/0xFF) & 0xFF;
            triplet[1] = (pixel/0xFFFF) & 0xFF;
            return triplet;
        }

        /** This sets the RGB sample at the given view coordinates. */
        public void PutSample(int x, int y, int[] newvals)
        {
            x = ImageX(x);
            y = ImageY(y);
            int offset = x + y * Width;
            int pixel = newvals[0] + (newvals[1]*0xFF) + (newvals[2]*0xFFFF);
            Image[offset] = pixel;
        }

        /* This converts view coordinates into image coordinates. */
        protected int ImageX(int x)
        {
            return x + XOffset;
        }

        /* This converts view coordinates into image coordinates. */
        protected int ImageY(int y)
        {
            return y + YOffset;
        }
    }
}