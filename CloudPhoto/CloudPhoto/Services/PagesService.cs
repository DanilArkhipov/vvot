namespace Cloudphoto.Services
{
    internal class PagesService
    {
        public static string GetAlbumPage(string content)
        {
            return "<!doctype html>" +
                "<html>" +
                "<head>" +
                "<meta charset=\"UTF-8\" />" +
                "<link rel=\"stylesheet\" " +
                "type=\"text/css\" " +
                "href=\"https://cdnjs.cloudflare.com/ajax/libs/galleria/1.6.1/themes/classic/galleria.classic.min.css\"/>" +
                "<style>" +
                ".galleria{ width: 960px; height: 540px; background: #000 }" +
                "</style>" +
                "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js\"></script>" +
                "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/galleria/1.6.1/galleria.min.js\"></script>" +
                "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/galleria/1.6.1/themes/classic/galleria.classic.min.js\"></script>" +
                "</head>" +
                "<body>" +
                "<div class=\"galleria\">" +
                content +
                "</div>" +
                "<p> Вернуться на <a href =\"index.html\">главную страницу</a> фотоархива.</p>" +
                "<script>" +
                "(function() {" +
                "Galleria.run('.galleria');" +
                "}());" +
                "</script>" +
                "</body>" +
                "</html>";
        }

        public static string GetIndexPage(string content)
        {
            return "<!doctype html>" +
                "<html>" +
                "<head>" +
                "<meta charset=\"UTF-8\"/>" +
                "<title>Фотоархив</title>" +
                "</head>" +
                "<body>" +
                "<h1>Фотоархив</h1>" +
                "<ul>" +
                content +
                "</ul>" +
                "</body>";
        }

        public static string GetErrorPage()
        {
            return "<!doctype html>" +
                "<html>" +
                "<head>" +
                "<meta charset=\"UTF-8\"/>" +
                "<title>Фотоархив</title>" +
                "</head>" +
                "<body>" +
                "<h1>Ошибка</h1>" +
                "<p>Ошибка при доступе к фотоархиву. " +
                "Вернитесь на <a href=\"index.html\">" +
                "главную страницу</a> фотоархива.</p>" +
                "</body>" +
                "</html>";
        }
    }
}