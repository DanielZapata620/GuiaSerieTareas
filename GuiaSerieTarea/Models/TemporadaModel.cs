using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiaSerieTarea.Models
{
    public class TemporadaModel
    {
        public int NumeroTemporada { get; set; }

        public int CantidadEpisodios=>Episodios.Count;
        public string Descripcion { get; set; } = null!;

        public DateTime FechaLanzamientoTemporada { get; set; }=DateTime.Now;

        public string Imagen { get; set; } = null!;

        public ObservableCollection<EpisodioModel> Episodios { get; set; } = new();
    }
}
