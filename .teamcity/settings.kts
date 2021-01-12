// ------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated.
//
//     - To turn off auto-generation set:
//
//         [TeamCity (AutoGenerate = false)]
//
//     - To trigger manual generation invoke:
//
//         nuke --generate-configuration TeamCity --host TeamCity
//
// </auto-generated>
// ------------------------------------------------------------------------------

import jetbrains.buildServer.configs.kotlin.v2018_1.*
import jetbrains.buildServer.configs.kotlin.v2018_1.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2018_1.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2018_1.triggers.*
import jetbrains.buildServer.configs.kotlin.v2018_1.vcs.*

version = "2020.2"

project {
    buildType(Down)
    buildType(Up)
    buildType(Compile)
    buildType(Test_P1T2)
    buildType(Test_P2T2)
    buildType(Test)

    buildTypesOrder = arrayListOf(Down, Up, Compile, Test_P1T2, Test_P2T2, Test)

    params {
        select (
            "env.Verbosity",
            label = "Verbosity",
            description = "Logging verbosity during build execution. Default is 'Normal'.",
            value = "Normal",
            options = listOf("Minimal" to "Minimal", "Normal" to "Normal", "Quiet" to "Quiet", "Verbose" to "Verbose"),
            display = ParameterDisplay.NORMAL)
        select (
            "env.Configuration",
            label = "Configuration",
            description = "Configuration to build - Default is 'Debug' (local) or 'Release' (server)",
            value = "Release",
            options = listOf("Debug" to "Debug", "Release" to "Release"),
            display = ParameterDisplay.NORMAL)
        checkbox (
            "env.WipeDatabaseData",
            label = "WipeDatabaseData",
            description = "Check both checkboxes to wipe the database data",
            value = "False",
            checked = "True",
            unchecked = "False",
            display = ParameterDisplay.NORMAL)
        checkbox (
            "env.WipeDatabaseDataProtection",
            label = "WipeDatabaseDataProtection",
            value = "False",
            checked = "True",
            unchecked = "False",
            display = ParameterDisplay.NORMAL)
    }
}
object Down : BuildType({
    name = "Down"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    steps {
        exec {
            path = "build.sh"
            arguments = "Down --skip"
        }
    }
})
object Up : BuildType({
    name = "Up"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    steps {
        exec {
            path = "build.sh"
            arguments = "Up --skip"
        }
    }
    triggers {
        vcs {
            triggerRules = "+:**"
        }
    }
    dependencies {
        snapshot(Down) {
            onDependencyFailure = FailureAction.FAIL_TO_START
            onDependencyCancel = FailureAction.CANCEL
        }
    }
})
object Compile : BuildType({
    name = "Compile"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    steps {
        exec {
            path = "build.sh"
            arguments = "Restore Compile --skip"
        }
    }
})
object Test_P1T2 : BuildType({
    name = "Test 1/2"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    artifactRules = "output/test-results/*.trx => output/test-results"
    steps {
        exec {
            path = "build.sh"
            arguments = "Test --skip --test-partition 1"
        }
    }
    dependencies {
        snapshot(Compile) {
            onDependencyFailure = FailureAction.FAIL_TO_START
            onDependencyCancel = FailureAction.CANCEL
        }
    }
})
object Test_P2T2 : BuildType({
    name = "Test 2/2"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    artifactRules = "output/test-results/*.trx => output/test-results"
    steps {
        exec {
            path = "build.sh"
            arguments = "Test --skip --test-partition 2"
        }
    }
    dependencies {
        snapshot(Compile) {
            onDependencyFailure = FailureAction.FAIL_TO_START
            onDependencyCancel = FailureAction.CANCEL
        }
    }
})
object Test : BuildType({
    name = "Test"
    type = Type.COMPOSITE
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
        showDependenciesChanges = true
    }
    artifactRules = "**/*"
    triggers {
        vcs {
            triggerRules = "+:**"
        }
    }
    dependencies {
        snapshot(Test_P1T2) {
            onDependencyFailure = FailureAction.ADD_PROBLEM
            onDependencyCancel = FailureAction.CANCEL
        }
        snapshot(Test_P2T2) {
            onDependencyFailure = FailureAction.ADD_PROBLEM
            onDependencyCancel = FailureAction.CANCEL
        }
        artifacts(Test_P1T2) {
            artifactRules = "**/*"
        }
        artifacts(Test_P2T2) {
            artifactRules = "**/*"
        }
    }
})
