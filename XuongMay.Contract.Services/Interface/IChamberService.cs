using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.ChamberModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IChamberService
	{
		Task ImportChambers(ImportModel model);
	}
}
