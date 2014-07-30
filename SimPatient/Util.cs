﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Globalization;

using Zen.Barcode;

//special namespaces and aliases
using Color = System.Drawing.Color;
using RotateFlipType = System.Drawing.RotateFlipType;
using DrawFont = System.Drawing.Font;
using DrawBrush = System.Drawing.Brush;
using DrawBrushes = System.Drawing.Brushes;

using System.Drawing.Imaging;
using Graphics = System.Drawing.Graphics;
using Bitmap = System.Drawing.Bitmap;
using DrawImage = System.Drawing.Image;
using System.Drawing.Drawing2D;
using RectangleF = System.Drawing.RectangleF;
using GraphicsUnit = System.Drawing.GraphicsUnit;

using System.Windows.Controls;
//add reference to Microsoft.mshtml assembly in order to access IHTMLDocument2
using mshtml;

using System.Text.RegularExpressions;

namespace SimPatient
{
	class Util
	{

		//---example
		//htmlFile = @"C:\Users\Steviesama\Dropbox\www-steve\robert\index.html"
		public static void printHtml(string htmlFile)
		{
			WebBrowser browser = new WebBrowser();
			browser.Navigate(htmlFile);

			(browser.Document as IHTMLDocument2).execCommand("PRINT", true, null);
		}
		
		public static DrawingVisual getDrawingVisual(BitmapImage bitmap)
		{
			var vis = new DrawingVisual();
			var dc = vis.RenderOpen();
			dc.DrawImage(bitmap, new Rect(0, 0, bitmap.Width, bitmap.Height));
			dc.Close();

			return vis;
		}

		public static void saveAsPng(RenderTargetBitmap src, Stream outputStream)
		{
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(src));
			encoder.Save(outputStream);
		}

		public static void saveAsPng(BitmapImage src, Stream outputStream)
		{
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(src));
			encoder.Save(outputStream);
		}

		public static void saveAsBmp(RenderTargetBitmap src, Stream outputStream)
		{
			TiffBitmapEncoder encoder = new TiffBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(src));

			encoder.Save(outputStream);
		}

		private static  Bitmap getBitmap(BitmapImage bitmapImage)
		{
			// BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

			using (MemoryStream outStream = new MemoryStream())
			{
				//BitmapEncoder enc = new BmpBitmapEncoder();
				//enc.Frames.Add(BitmapFrame.Create(bitmapImage));
				//enc.Save(outStream);
				saveAsPng(getImage(getDrawingVisual(bitmapImage)), outStream);
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}

		public static FormattedText createFormattedText(string text, double emSize, Brush textColor)
		{
			FormattedText fText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), emSize, textColor);
			return fText;
		}

		public static BitmapImage getBitmapImage(Bitmap bmp)
		{
			//create memory stream for barcode conversion
			MemoryStream ms = new MemoryStream();
			//save barcode image to memory stream as PNG
			bmp.Save(ms, ImageFormat.Png);
			//set memory stream position back to the beginning
			ms.Position = 0;
			//create bitmap image
			BitmapImage bitmapImage = new BitmapImage();
			//prep bitmap for memory stream
			bitmapImage.BeginInit();
			//set memory stream
			bitmapImage.StreamSource = ms;
			//end stream prep
			bitmapImage.EndInit();
			//return BitmapImage
			return bitmapImage;
		}

		public static DrawingVisual renderPatientBarcode(Patient patient)
		{            
			BitmapImage barcodeBitmap = null;

			//render barcode image
			barcodeBitmap = renderCode128("MR" + patient.Id);

			//---render patient text information around barcode image

			double fontSize = 14;
			var vis = new DrawingVisual();
			var dc = vis.RenderOpen();
			
			string txt = "Name: " + patient.Name + "\nDOB: " + patient.DateOfBirth.ToString("MM/dd/yyyy") +
						 "  Gender: " + (patient.Gender == Patient.PatientGender.Male ? "Male" : "Female") +
						 "\n" + patient.DrName + " - MR" + patient.Id;
			//dummy text to anchor the top-left corner so translation works for subsequent draws
			FormattedText fText = createFormattedText(txt, fontSize, Brushes.White);
			fText.MaxTextWidth = 350;
			fText.MaxTextHeight = 100;

			dc.DrawText(fText, new Point(0, 0));
			fText = createFormattedText(txt, fontSize, Brushes.Black);
			fText.MaxTextWidth = 350;
			fText.MaxTextHeight = 100;
			dc.DrawText(fText, new Point(50, 50));
			
			dc.DrawImage(barcodeBitmap, new Rect(75, 105, barcodeBitmap.Width, barcodeBitmap.Height));
			
			dc.Close();

			return vis;
		}

		public static DrawingVisual renderMedicationBarcode(Medication medication)
		{
			BitmapImage barcodeBitmap = null;

			//render barcode image
			barcodeBitmap = renderCode128("MED" + medication.Id);

			//---render patient text information around barcode image

			double fontSize = 14;
			var vis = new DrawingVisual();
			var dc = vis.RenderOpen();

			string txt = "\nName: " + medication.Name + "\n" + medication.Strength + "  -  MED" + medication.Id;
			//dummy text to anchor the top-left corner so translation works for subsequent draws
			FormattedText fText = createFormattedText(txt, fontSize, Brushes.White);
			fText.MaxTextWidth = 350;
			fText.MaxTextHeight = 100;

			dc.DrawText(fText, new Point(0, 0));
			fText = createFormattedText(txt, fontSize, Brushes.Black);
			fText.MaxTextWidth = 350;
			fText.MaxTextHeight = 100;
			dc.DrawText(fText, new Point(50, 50));

			dc.DrawImage(barcodeBitmap, new Rect(75, 105, barcodeBitmap.Width, barcodeBitmap.Height));

			dc.Close();

			return vis;
		}

		public static BitmapImage renderCode128(string text)
		{
			BitmapImage bitmap;
			//create barcode image
			Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
			//draw barcode image
			DrawImage image = barcode.Draw(text, 40, 2);
			//create memory stream for barcode conversion
			MemoryStream ms = new MemoryStream();
			//save barcode image to memory stream as PNG
			image.Save(ms, ImageFormat.Png);
			//---debug render to file
			//FileStream fStream = new FileStream("barcode.png", FileMode.Create);
			//image.Save(fStream, ImageFormat.Png);
			//set memory stream position back to the beginning
			ms.Position = 0;
			//create bitmap image
			bitmap = new BitmapImage();
			//prep bitmap for memory stream
			bitmap.BeginInit();
			//set memory stream
			bitmap.StreamSource = ms;
			//end stream prep
			bitmap.EndInit();

			return bitmap;
		}

		public static Bitmap cropBitmap(Bitmap bmp)
		{

			int w = bmp.Width;
			int h = bmp.Height;

			Func<int, bool> allWhiteRow = row =>
			{
				System.Drawing.Color color;
				for (int i = 0; i < w; ++i)
				{
					color = bmp.GetPixel(i, row);
					if (color.A != 0/*color.R != 255 && color.G != 255 && color.B != 255*/)
						return false;
				}
				return true;
			};

			Func<int, bool> allWhiteColumn = col =>
			{
				System.Drawing.Color color; ;
				for (int i = 0; i < h; ++i)
				{
					color = bmp.GetPixel(col, i);
					if (color.A != 0/*color.R != 255 && color.G != 255 && color.B != 255*/)
						return false;
				}
				return true;
			};

			int topmost = 0;
			for (int row = 0; row < h; ++row)
			{
				if (allWhiteRow(row))
					topmost = row;
				else break;
			}

			int bottommost = 0;
			for (int row = h - 1; row >= 0; --row)
			{
				if (allWhiteRow(row))
					bottommost = row;
				else break;
			}

			int leftmost = 0, rightmost = 0;
			for (int col = 0; col < w; ++col)
			{
				if (allWhiteColumn(col))
					leftmost = col;
				else break;
			}

			for (int col = w - 1; col >= 0; --col)
			{
				if (allWhiteColumn(col))
					rightmost = col;
				else break;
			}

			if (rightmost == 0) rightmost = w; // As reached left
			if (bottommost == 0) bottommost = h; // As reached top.

			int croppedWidth = rightmost - leftmost;
			int croppedHeight = bottommost - topmost;

			if (croppedWidth == 0) // No border on left or right
			{
				leftmost = 0;
				croppedWidth = w;
			}

			if (croppedHeight == 0) // No border on top or bottom
			{
				topmost = 0;
				croppedHeight = h;
			}

			try
			{
				var target = new Bitmap(croppedWidth, croppedHeight);
				using (Graphics g = Graphics.FromImage(target))
				{
					g.DrawImage(bmp,
					  new RectangleF(0, 0, croppedWidth, croppedHeight),
					  new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
					  GraphicsUnit.Pixel);
				}
				return target;
			}
			catch (Exception ex)
			{
				throw new Exception(
				  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
				  ex);
			}

		} //End cropBitmap()

		private static Bitmap padBitmap(Bitmap bmp, double padScale)
		{
			int widthShim = (int)(bmp.Width * padScale);
			int heightShim = (int)(bmp.Height * padScale);
			int newWidth = bmp.Width + widthShim;
			int newHeight = bmp.Height + heightShim;

			var target = new Bitmap(newWidth, newHeight);
			using (Graphics g = Graphics.FromImage(target))
			{
				g.DrawImage(bmp,
				  new RectangleF(widthShim / 2, heightShim / 2, bmp.Width, bmp.Height),
				  new RectangleF(0, 0, bmp.Width, bmp.Height),
				  GraphicsUnit.Pixel);
			}

			return target;
		}

		private static Bitmap resizeBitmap(Bitmap bitmap, Size size)
		{
			Bitmap b = new Bitmap(640, 480);
			Graphics gfx = Graphics.FromImage(b);
			gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

			return b;
		}

		private static DrawImage resizeImage(DrawImage imgToResize, Size size)
		{
			int sourceWidth = imgToResize.Width;
			int sourceHeight = imgToResize.Height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)size.Width / (float)sourceWidth);
			nPercentH = ((float)size.Height / (float)sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap b = new Bitmap(destWidth, destHeight);
			Graphics g = Graphics.FromImage((DrawImage)b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;

			g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
			g.Dispose();

			return (DrawImage)b;
		}

		/// <summary>
		/// Issue for BitmapSource returned and used as ImageSource for Image control.  Clipping occurs.
		/// </summary>
		private static RenderTargetBitmap renderBitmapSource(DrawingVisual drawingVisual)
		{
			RenderTargetBitmap bmp = new RenderTargetBitmap((Int32)Math.Ceiling(drawingVisual.ContentBounds.Width),
															(Int32)Math.Ceiling(drawingVisual.ContentBounds.Height),
															300, 300,
															PixelFormats.Pbgra32);

			bmp.Render(drawingVisual);
			return bmp;
		}

		public static RenderTargetBitmap getImage(DrawingVisual drawingVisual)
		{
			Size size = new Size(drawingVisual.ContentBounds.Width, drawingVisual.ContentBounds.Height);
			if (size.IsEmpty)
				return null;

			RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

			result.Render(drawingVisual);

			return result;
		}

		private static BitmapImage bitmapImageFromStream(Stream stream)
		{
			//create bitmap image
			BitmapImage bmp = new BitmapImage();
			//prep bitmap for memory stream
			bmp.BeginInit();
			//set memory stream
			bmp.StreamSource = stream;
			//end stream prep
			bmp.EndInit();
			//return BitmapImage
			return bmp;
		}
		
		#region Control Validation Functions

		public static bool validateStringTextBox(TextBox textBox)
		{
			bool isValid = true;
			textBox.Text = textBox.Text.Trim();

			if (textBox.Text == string.Empty)
			{
				isValid = false;
				textBox.Background = Brushes.LightPink;
			}
			else textBox.Background = Brushes.White;

			return isValid;
		}

		public static bool validateTimeTextBox(TextBox textBox)
		{
			bool isValid = true;
			textBox.Text = textBox.Text.Trim();
			
			int num = 0;

			//if number format isn't exactly 4 characters
			if (textBox.Text.Length != 4)
				isValid = false;
			//else if text is not a valid integer value
			else if (int.TryParse(textBox.Text, out num) == false)
				isValid = false;
			//else if the number isn't within a valid 24 hour time range
			else if (num < 0 || num > 2359)
				isValid = false;

			//if input isn't valid, set background of control to indicate this
			if (isValid == false) textBox.Background = Brushes.LightPink;
			//else ensure the background is set to a non-erroneous visual
			else textBox.Background = Brushes.White;

			return isValid;
		}

		public static bool validateMRTextBox(TextBox textBox)
		{
			bool success = false;
			textBox.Text = textBox.Text.Trim();

			if (textBox.Text == string.Empty)
			{
				MessageBox.Show("Auto-id is not checked, please enter a unique Medical Record Number.",
								"Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				success = false;
			}

			success = validateMedicalRecordNumber(textBox.Text);

			if (success == false)
			{
				MessageBox.Show(string.Format("'{0}' is not a valid Medical Record Number.\n" +
											  "Please use MR123456 as the format.\n" +
											  "MR in caps, no space, followed by at least 6 digits, but no more than 9.", textBox.Text),
											  "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				textBox.Background = Brushes.LightPink;
			}
			else textBox.Background = Brushes.White;

			return success;
		}

		public static bool validateMedicalRecordNumber(string mrNumber)
		{
			return Regex.Match(mrNumber, @"^MR\d{6,9}?$").Success;
		}

		public static bool validateNumberTextBox(TextBox textBox)
		{
			bool isValid = true;
			double number;
			textBox.Text = textBox.Text.Trim();

			if (double.TryParse(textBox.Text, out number) == false)
			{
				isValid = false;
				textBox.Background = Brushes.LightPink;
			}
			else textBox.Background = Brushes.White;

			return isValid;
		}

		public static bool validateDatePicker(DatePicker datePicker)
		{
			bool isValid = true;

			if (datePicker.SelectedDate == null)
			{
				isValid = false;
				datePicker.Background = Brushes.LightPink;
			}
			else datePicker.Background = Brushes.White;

			return isValid;
		}

		#endregion Control Validation Functions
	} //End class Util
} //End namespace SimPatient