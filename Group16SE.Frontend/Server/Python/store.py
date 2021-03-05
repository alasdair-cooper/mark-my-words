import pandas
import logging
import os
import bson
import json


class FileErr(Exception):
    pass

class fileMangagement:

    def openJSON(self, name):
        PROJECT_ROOT_DIR = "."
        CHAPTER_ID = "local_assignment_content"
        base_id = "local_storage_interaction"
        save_path = os.path.join(base_id, "data", CHAPTER_ID, name)

        try:
            with open(save_path) as json_file:
                data = json.load(json_file)
            return data

        except:
            return {"file":"not found"}

    def saveJSON(self, name, contents, subfolderName=None):
        import os.path

        PROJECT_ROOT_DIR = "."
        CHAPTER_ID = "local_assignment_content"

        if subfolderName == None:
            save_path = os.path.join(PROJECT_ROOT_DIR, "data", CHAPTER_ID)
        else:
            save_path = os.path.join(PROJECT_ROOT_DIR, "data", CHAPTER_ID, subfolderName)

        os.makedirs(save_path, exist_ok=True)

        completeName = os.path.join(save_path, name + ".json")

        with open(completeName, "w", encoding='utf-8') as file:
            try:
                json.dump(contents, file)
            except Exception as e:
                raise FileErr() from e

    def delJSON(self, name):
        PROJECT_ROOT_DIR = "."
        CHAPTER_ID = "local_assignment_content"

        save_path = os.path.join(PROJECT_ROOT_DIR, "data", CHAPTER_ID, name)
        completeName = os.path.join(save_path, name + ".json")
        os.remove(completeName)





class assignment:
    def insertOne(self, assignment):
        json_assignment = json.load(assignment)
        identifier = json_assignment["AssignmentId"]

        try:
            fileMangagement().saveJSON(identifier, json_assignment)
            return 1
        except:
            fileMangagement().delJSON(identifier)
            self.insertOne(assignment)
            return 2


    def getOne(self, identifier):
        if identifier.split(".")[-1] != "json":
            identifier = identifier + ".json"
        return fileMangagement().openJSON(identifier)


if __name__ == '__main__':
    with open('assighnment.json', "r") as json_file:
        assignment().insertOne(json_file)
        print(assignment().getOne("730aec0f-a23f-478e-9c94-787f9ab60526"))
