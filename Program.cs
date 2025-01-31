using System.Linq.Expressions;

class Program
{
    static bool quitter = false;
    static void Main(string[] args)
    {

        while (!quitter)
        {
            AfficherMenu();
            Console.Write("Votre choix : ");
            ConsoleKeyInfo choix = Console.ReadKey();

            switch (choix.KeyChar)
            {
                case '1':
                    CreerSauvegarde();
                    break;

                case '2':
                    AfficherSauvegarde();
                    break;

                case '3':
                    EditerSauvegarde();
                    break;

                case '4':
                    SupprimerSauvegarde();
                    break;

                case '5':
                    Quitter();
                    break;
            }

            Console.WriteLine();
            Console.ReadKey();
        }

    }

    static void AfficherMenu()
    {
        Console.WriteLine("==========================================================\n");
        Console.WriteLine("Bienvenue dans l'application de gestion de sauvegardes");
        Console.WriteLine("1. Créer une sauvegarde");
        Console.WriteLine("2. Afficher une sauvegarde");
        Console.WriteLine("3. Éditer une sauvegarde");
        Console.WriteLine("4. Supprimer une sauvegarde");
        Console.WriteLine("5. Quitter\n");
        Console.WriteLine("==========================================================\n");
        Console.WriteLine("Appuyez sur une touche pour continuer\n\n");
    }

    static void CreerSauvegarde()
    {
        Console.Clear();
        Console.WriteLine("Création d'une sauvegarde");
    }

    static void AfficherSauvegarde()
    {
        Console.Clear();
        Console.WriteLine("Affichage d'une sauvegarde");
    }

    static void EditerSauvegarde()
    {
        Console.Clear();
        Console.WriteLine("Édition d'une sauvegarde");
    }

    static void SupprimerSauvegarde()
    {
        Console.Clear();
        Console.WriteLine("Suppression d'une sauvegarde");
    }

    static void Quitter()
    {
        quitter = true;
        Console.WriteLine("\n\nAu revoir !");

    }
}