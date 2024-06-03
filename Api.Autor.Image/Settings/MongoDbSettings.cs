namespace Api.Autor.Image.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; } // Nombre de la colección para imágenes
    }
}
