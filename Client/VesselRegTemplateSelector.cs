using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ANWI;

namespace Client {
	/*public class VesselRegTemplateSelector : DataTemplateSelector {
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			FrameworkElement element = container as FrameworkElement;

			if(element != null && item != null && item is VesselReg.VesselRecord) {
				if (item is VesselReg.NamedVessel) {
					return element.FindResource("NamedTemplate") as DataTemplate;
				} else if(item is VesselReg.Divider) {
					return element.FindResource("DividerTemplate") as DataTemplate;
				} else {
					return null;
				}
			}

			return null;
		}
	}*/
}
