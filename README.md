The project follows a clean architecture approach with the following layers:

- Domain: Contains entities, enums, and repository interfaces
- Application: Contains DTOs, services, validators, and business logic
- Infrastructure: Contains database context, repository implementations, and dependency injection
- Presentation: Contains controllers and API endpoints

***Please Read Documentation.txt file for more informations about the app logic & Self-Review.txt for my suggested improvements***

Live Demo
The application is deployed and accessible on Render Cloud at:

https://leave-management-system-uxmh.onrender.com/swagger

Local Running Using Docker : 

 - Clone the repository (git clone https://github.com/ismailsahnoun/leave-management-system.git)
 - Navigate to the project directory ( cd leave-management-system ) 
 - Build the docker image ( sudo docker-compose build )  
 - Run the docker image: ( sudo docker-compose up -d) 
 - navigate on your browser to http://localhost:5000/swagger/
