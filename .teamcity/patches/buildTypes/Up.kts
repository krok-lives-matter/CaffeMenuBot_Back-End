package patches.buildTypes

import jetbrains.buildServer.configs.kotlin.v2018_1.*
import jetbrains.buildServer.configs.kotlin.v2018_1.buildSteps.ExecBuildStep
import jetbrains.buildServer.configs.kotlin.v2018_1.buildSteps.exec
import jetbrains.buildServer.configs.kotlin.v2018_1.ui.*

/*
This patch script was generated by TeamCity on settings change in UI.
To apply the patch, change the buildType with id = 'Up'
accordingly, and delete the patch script.
*/
changeBuildType(RelativeId("Up")) {
    expectSteps {
        exec {
            path = "build.sh"
            arguments = "Up --skip"
        }
    }
    steps {
        insert(0) {
            step {
                name = "Production docker-compose configuration"
                type = "MRPP_CreateTextFile"
                param("system.dest.file", "docker-compose.override.yml")
                param("content", """
                    version: '3.7'
                    
                    services:
                      host:
                        environment:
                          - ASPNETCORE_ENVIRONMENT=Production
                      postgres:
                        environment:
                          - POSTGRES_PASSWORD=quc45iVMHo3pJG2kXswvfmkU
                """.trimIndent())
            }
        }
        insert(1) {
            step {
                name = "Production database settings"
                type = "MRPP_CreateTextFile"
                param("system.dest.file", "dbsettings.Production.json")
                param("content", """
                    {
                        "ConnectionStrings": {
                            "CaffeMenuBotDb": "Host=caffe_menu_bot_postgres;Port=5432;UserId=postgres;Password=quc45iVMHo3pJG2kXswvfmkU;Database=caffe_menu_bot;CommandTimeout=300;"
                        }
                    }
                """.trimIndent())
            }
        }
        update<ExecBuildStep>(2) {
            clearConditions()
        }
    }
}
