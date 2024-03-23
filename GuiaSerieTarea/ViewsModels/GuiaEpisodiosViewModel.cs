using GalaSoft.MvvmLight.Command;
using GuiaSerieTarea.Models;
using GuiaSerieTarea.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiaSerieTarea.ViewsModels
{
    public enum Ventanas { Principal,Agregar,AgregarEpidosio,EditarTemporada,EditarEpisodio,EliminarEpisodio,EliminarTemporada,Episodios}
    public class GuiaEpisodiosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<TemporadaModel> Temporadas { get; set; } = new();
        public Ventanas Ventana { get; set; }=Ventanas.Principal;

        public ICommand CambiarVistaCommand { get; set; }
        
        public ICommand AgregarCommand { get; set; }

        public ICommand AgregarEpidosioCommand { get; set; }

        public ICommand EditarTemporadaCommand {  get; set; }

        public ICommand EditarEpisodioCommand {  get; set; }

        public ICommand EliminarEpisodioCommand { get; set; }

        public ICommand EliminarTemporadaCommand { get; set; }
        public TemporadaModel? Temporada { get; set; }

        public EpisodioModel? Episodio { get; set; }
        public int CantidadTemporadas=>Temporadas.Count;

        public string? TextoBoton { get;set; }

        public string Error { get; set; } = "";

        
        public GuiaEpisodiosViewModel()
        {
            CambiarVistaCommand = new RelayCommand<string>(CambiarVista);
            
            AgregarCommand = new RelayCommand(AgregarTemporada);
            AgregarEpidosioCommand = new RelayCommand(AgregarEpisodio);
            EditarTemporadaCommand = new RelayCommand(GuardarTemporada);
            EditarEpisodioCommand = new RelayCommand(GuardarEpisodio);
            EliminarEpisodioCommand=new RelayCommand(EliminarEpisodio);
            EliminarTemporadaCommand=new RelayCommand(EliminarTemporada);
            Deserializar();
        }

        private void EliminarTemporada()
        {
            if (Temporada != null)
            {
                Temporadas.Remove(Temporada);
                OrdenarLista();
                Serializar();
                Ventana=Ventanas.Principal;
                Actualizar(nameof(Ventana));

            }
        }

        private void EliminarEpisodio()
        {
            if (Episodio != null&&Temporada!=null)
            {
                Temporada.Episodios.Remove(Episodio);
                OrdenarLista();
                Serializar();
                Ventana = Ventanas.Episodios;
                Actualizar(nameof(Ventana));

            }
        }

        void ValidarInfoTemporada()  
        {

            if (Temporada != null)
            {
                if (Temporada.NumeroTemporada <= 0)
                {
                    Error += "Errores: Ingrese el numero de temporada  |  ";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Descripcion))
                {
                    Error += "Ingrese la descripcion de a temporada  |  ";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Imagen) || !Temporada.Imagen.StartsWith("http") || !Temporada.Imagen.EndsWith(".jpg"))
                {
                    Error += "Ingrese a URL de a temporada  |  ";
                }
                if (Temporada.FechaLanzamientoTemporada > DateTime.Now)
                {
                    Error += "Verifique la fecha ingresada  |  ";
                }
               

                Actualizar(nameof(Error));
            }
        }

        void ValidarInfoEpisodio()
        {

            if (Episodio != null)
            {
                if (string.IsNullOrWhiteSpace(Episodio.Nombre))
                {
                    Error += "Errores: Ingrese el nombre del episodio  |  ";
                }
                if (Episodio.DuracionMinutos <= 0)
                {
                    Error += "Verifique la duracion en minutos  |  ";
                }
                if (string.IsNullOrWhiteSpace(Episodio.DescripcionEpisodio))
                {
                    Error += "Ingrese la descripcion del episodio  |  ";
                }
                if (string.IsNullOrWhiteSpace(Episodio.ImagenEpisodioo) || !Episodio.ImagenEpisodioo.StartsWith("http") || !Episodio.ImagenEpisodioo.EndsWith(".jpg"))
                {
                    Error += "Ingrese la URL de la imagen del episodio  |  ";
                }
                if (Episodio.FechaLanzamientoEpisodio > DateTime.Now)
                {
                    Error += "Verifique la fecha ingresada  |  ";
                }
                

                Actualizar(nameof(Error));
            }
        }

        void OrdenarLista()  //Ordena la lista segun el numero de temporada
        {   
            var TemporadasListaOrdenada=Temporadas.OrderBy(x => x.NumeroTemporada).ToList();
            Temporadas = new ObservableCollection<TemporadaModel> (TemporadasListaOrdenada);
        }
       
        private void GuardarEpisodio()  //Edita el episodio
        {
            Error = "";
            if (Error == "" && Temporada != null && Episodio!=null)
            {
                ValidarInfoEpisodio();

                if (Error == "")
                {
                    Temporada.Episodios[posAnterior] = Episodio;
                    OrdenarLista();
                    Serializar();
                    Ventana = Ventanas.Episodios;
                    Actualizar(nameof(Ventana));
                }
            }
        }

        private void GuardarTemporada()  //Edita la temporada
        {
            Error = "";
            if (Error == "" && Temporada!=null)
            {
                ValidarInfoTemporada();

               

                if (Error == "")
                {
                    Temporadas[posAnterior] = Temporada;
                    OrdenarLista();
                    Serializar();
                    Ventana = Ventanas.Principal;
                    Actualizar(nameof(Ventana));
                }
            }
        }

        private void AgregarEpisodio()
        {
            Error = "";
            if (Temporada != null )
            {
                
                if (Temporadas.Any(x => x.Episodios.Any(e => e.Nombre == Episodio.Nombre)))
                {
                    Error += "El episodio ya existe  |  ";
                }
                ValidarInfoEpisodio();

                if (Error == "")
                {
                    Temporada.Episodios.Add(Episodio);
                    OrdenarLista();
                    //EpisodiosAMostrar=Temporada.Episodios;
                    Serializar();
                    Ventana = Ventanas.Episodios;
                    Actualizar(nameof(Ventana));
                    Actualizar(nameof(Temporada.Episodios));
                }
            }
        }


        

        private void AgregarTemporada()
        {
            Error = "";
            if (Temporada != null)
            {
                
                if (Temporadas.Any(x=>x.NumeroTemporada==Temporada.NumeroTemporada))
                {
                    Error += "La temporada ya existe  |  ";
                }
                ValidarInfoTemporada();

                if (Error == "") 
                { 
                Temporadas.Add(Temporada);
                OrdenarLista();
                Serializar();
                Ventana = Ventanas.Principal;
                Actualizar(nameof(Ventana));
                }
            
            }
        }

     

        private void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        int posAnterior;
        private void CambiarVista(string texto)  //Cambia de vista mandando un parametro obtenido del tag de cada boton segun la vista a cambiar
        {
            if (texto == "AgregarTemporada")
            {
            Temporada = new();
            Ventana=Ventanas.Agregar;
            
            }
            else if(texto=="AgregarEpisodio")
            {
                Episodio= new();
                Ventana = Ventanas.AgregarEpidosio;
            }
            else if (texto == "VerEpisodios")
            {
                Ventana = Ventanas.Episodios;
            }
            else if(texto == "RegresarPrincipal")
            {   
                Temporada = null;
                Ventana = Ventanas.Principal;
                
            }
            else if (texto == "RegresarEpisodios")
            {
                Episodio = null;
                Ventana= Ventanas.Episodios;
            }
            else if (texto == "VerEditarTemporada")
            {
                if (Temporada != null)
                {
                    var clon=new TemporadaModel
                    {
                        NumeroTemporada=Temporada.NumeroTemporada,
                        Descripcion=Temporada.Descripcion,
                        FechaLanzamientoTemporada=Temporada.FechaLanzamientoTemporada,
                        Imagen=Temporada.Imagen,
                        Episodios=Temporada.Episodios,
                    };
                    posAnterior=Temporadas.IndexOf(Temporada);
                    Temporada=clon;
                    Ventana = Ventanas.EditarTemporada;

                }
            }
            else if (texto == "VerEditarEpisodio")
            {
                if (Episodio != null && Temporada!=null)
                {
                    var clon = new EpisodioModel
                    {
                        Nombre=Episodio.Nombre,
                        DescripcionEpisodio=Episodio.DescripcionEpisodio,
                        FechaLanzamientoEpisodio =Episodio.FechaLanzamientoEpisodio,
                        ImagenEpisodioo=Episodio.ImagenEpisodioo,
                        DuracionMinutos=Episodio.DuracionMinutos
                    };
                    posAnterior=Temporada.Episodios.IndexOf(Episodio);
                    Episodio=clon;
                    Ventana=Ventanas.EditarEpisodio;
                }
            }
            else if (texto == "VerEliminarTemporada")
            {
                Ventana = Ventanas.EliminarTemporada;
            }
            else if(texto=="VerEliminarEpisodio")
            {
                Ventana = Ventanas.EliminarEpisodio;
            }
            Actualizar(nameof(Ventana));
        }

        void Serializar()
        {
            
            File.WriteAllText("Temporadas.json", JsonSerializer.Serialize(Temporadas));
        }

        void Deserializar()
        {
            try
            {
                var datos = JsonSerializer.Deserialize<ObservableCollection<TemporadaModel>>(File.ReadAllText("Temporadas.json"));
                if (datos != null)
                {
                    foreach (var temporada in datos)
                    {

                        Temporadas.Add(temporada);

                    }
                }
            }
            catch { }
        }
       
    }
}
