using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiaSerieTarea.Models
{
    public class EpisodioModel
    {
        public string Nombre { get; set; } = null!;

        
        public string DescripcionEpisodio { get; set; } = null!;
        public int DuracionMinutos { get; set; }

        public DateTime FechaLanzamientoEpisodio { get; set; }=DateTime.Now;

        public string ImagenEpisodioo { get; set; } = null!;
    }
}
