# Atuvu - Pooling
## Installation (To Use Package)
- In Packages/manifest.json, Add:
```
"dependencies": {
       "com.atuvu.pooling": "https://github.com/Qc-Leblond/Atuvu-Pooling.git",
        ...other unit packages
}
```

## Installation (To Edit Package)
- Pull latest master
- Add the path of the repository in the Packages/manifest.json:
```
"dependencies": {
       "com.atuvu.pooling": "REPOSITORY_PATH",
        ...other unit packages
}
```

## Running Tests
- In Packages/manifest.json, Add:
```
"dependencies": {
       "com.atuvu.pooling": "REPOSITORY_PATH",
        ...other unit packages
},
"testables": [
      "com.atuvu.pooling"
]
```
- In Unity, Open TestRunner in Window/General/Test Runner
- In the test runner, switch to Playmode
- Click run all or run all in player
- For more details: https://docs.unity3d.com/Manual/testing-editortestsrunner.html

There is also performance tests. Details on how to see them can be found here https://docs.unity3d.com/Packages/com.unity.test-framework.performance@0.1/manual/index.html
