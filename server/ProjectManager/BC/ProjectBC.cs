﻿using System.Collections.Generic;
using System.Linq;
using MODEL = ProjectManager.Models;
using DAC = ProjectManager.DAC;

namespace ProjectManager.BC
{
    public class ProjectBC
    {
        DAC.ProjectManagerEntities dbContext = null;
        public ProjectBC()
        {
            dbContext = new DAC.ProjectManagerEntities();
        }

        public ProjectBC(DAC.ProjectManagerEntities context)
        {
            dbContext = context;
        }
        public List<MODEL.Project> RetrieveProjects()
        {
            using (dbContext)
            {
                return dbContext.Projects.Select(x => new MODEL.Project()
                {
                    ProjectId = x.Project_ID,
                    ProjectName = x.Project_Name,
                    ProjectEndDate = x.End_Date,
                    ProjectStartDate = x.Start_Date,
                    Priority = x.Priority,                 
                    User = x.Manager.HasValue ? new MODEL.User(){UserId = x.Manager.Value} : null,                    
                    NoOfTasks = dbContext.Tasks.Where(y => y.Project_ID == x.Project_ID).Count(),
                    NoOfCompletedTasks = dbContext.Tasks.Where(y => y.Project_ID == x.Project_ID && y.Status == 1).Count(),
                }).ToList();
            }
        }

        public int InsertProjectDetails(MODEL.Project project)
        {
            using (dbContext)
            {
                DAC.Project proj = new DAC.Project()
                {
                    Project_Name = project.ProjectName,
                    Start_Date = project.ProjectStartDate,
                    End_Date = project.ProjectEndDate,
                    Priority = project.Priority,
                    Manager = project.User.UserId
                };
                dbContext.Projects.Add(proj);
                return dbContext.SaveChanges();
                
            }
        }

        public int UpdateProjectDetails(MODEL.Project project)
        {
            using (dbContext)
            {
                var editProjDetails = (from editProject in dbContext.Projects
                                       where editProject.Project_ID.ToString().Contains(project.ProjectId.ToString())
                                       select editProject).First();
                // Modify existing records
                if (editProjDetails != null)
                {
                    editProjDetails.Project_Name = project.ProjectName;
                    editProjDetails.Start_Date = project.ProjectStartDate;
                    editProjDetails.End_Date = project.ProjectEndDate;
                    editProjDetails.Priority = project.Priority;
                    editProjDetails.Manager = project.User.UserId;
                }


                
                return dbContext.SaveChanges();
            }

        }
        public int DeleteProjectDetails(MODEL.Project project)
        {
            using (dbContext)
            {

                var editDetails = (from proj in dbContext.Projects
                                   where proj.Project_ID == project.ProjectId
                                   select proj).First();
                // Delete existing record
                if (editDetails != null)
                {
                    dbContext.Projects.Remove(editDetails);
                }
                return dbContext.SaveChanges();
            }

        }

    }
}