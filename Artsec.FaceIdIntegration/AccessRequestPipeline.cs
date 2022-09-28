using Artsec.PassController.Domain.Requests;
using PipeLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artsec.PassController;

public class PassRequestPipeline : Pipeline<PassRequest>
{
	public PassRequestPipeline()
	{

	}
}
