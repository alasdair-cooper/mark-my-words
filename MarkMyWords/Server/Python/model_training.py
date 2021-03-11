from data_preparation import *
import numpy as np
import sys


class DfManipulation:
    def dropNonCompleatedsections(self, dfs):
        """
        goes though each of the dataframes and finds each record that has been marked as completed
        returns a modified dict of the dataframes with the non complete records removed
        """
        newdf= {}
        for key in dfs:
            df = dfs[key]
            df = df.drop(df[df.finished != 1].index)
            del df['finished']
            newdf[key] = df

        return newdf

    def makeDataFrame(self, JSONassighnment):
        DFs = createDataFrames(JSONassighnment)
        return DFs

    def splitData(self, dataFrame, ratio):
        from sklearn.model_selection import train_test_split
        return train_test_split(dataFrame.drop(['mark'], axis = 'columbs'), dataFrame.mark, test_size= ratio)

    def spplitSectionLabeling(self, dfs):
        newdf = {}
        for key in dfs:
            df = dfs[key]

            feature_df, mark_df = df.drop(['mark'], axis=1), pd.DataFrame(df.mark)
            newdf[key] = {"features": feature_df, "labels": mark_df}

        return newdf

class ModelManipulation:

    def makeFreshRF(self, trees = 30):
        from sklearn.ensemble import RandomForestRegressor
        return RandomForestRegressor(n_estimators=trees)

    def trainModel(self, model, train_x, train_y):
        return model.fit(train_x, train_y)

    def queryModel(self, model, query_data_frame):
        return model.predict(query_data_frame)[0]

    def createRFmodel(self, x_train, y_train, tree_num):

        model = self.makeFreshRF(tree_num)
        self.trainModel(model, x_train, list(y_train['mark']))

        return model

    def createModlesForSections(self, dfs, tree_num, specificSection = False):
        newdf= {}
        for key in dfs:
            features = dfs[key]["features"]
            labels = dfs[key]["labels"]

            model = self.createRFmodel(features, labels, tree_num)

            if specificSection == False:
                newdf[key] = model
            elif key == specificSection:
                newdf[key] = model

        return newdf

    def runOnSelf(self, features, model):
        predicted_labels = model.predict(features)
        return predicted_labels

    def measureError(self, test_features, test_labels, model):
        from sklearn import metrics

        predicted_labels = self.runOnSelf(test_features, model)

        MAE = metrics.mean_absolute_error(test_labels, predicted_labels)
        RMSE = np.sqrt(metrics.mean_squared_error(test_labels, predicted_labels))

        return MAE, RMSE

def cmdLineMain():
    args = sys.argv
    AssignmentPath, SectionId, AttemptId = args[1], args[2], args[3]

    try:
        with open(AssignmentPath) as f:
            content  = json.load(f)
    except:
        print("file not found")
        exit(1)

    model = ModelManipulation()
    dataFrame = DfManipulation()

    # preparing dataframes
    dfs = createDataFrames(content)
    q_df = createDataFrames(content,  atttemptID = AttemptId)

    # gets rid of sections in attepts that ahve not been marked as compleated
    filtered_dfs = dataFrame.dropNonCompleatedsections(dfs)

    # splits the feature and label parts
    split_dfs = dataFrame.spplitSectionLabeling(filtered_dfs)
    q_split_df = dataFrame.spplitSectionLabeling(q_df)[SectionId]['features'].drop(["finished"], axis=1)

    if len(split_dfs) > 10:

        # create models for each section
        tree_num = 30
        model_dict = model.createModlesForSections(split_dfs, tree_num=tree_num, specificSection=SectionId)

        #querys the model with the section of the attempt
        q_answer = model.queryModel(model_dict[SectionId], q_split_df)

        #calculates accuracey by testing feature vecs on the model and finding RMSE and MAE
        acc = model.measureError(split_dfs[SectionId]["features"], split_dfs[SectionId]["labels"], model_dict[SectionId])


        return ("{\"min\":%s,\"max\":%s}" % (q_answer - acc[0], q_answer + acc[0]))

    else:
        return ("{\"min\":%s,\"max\":%s}" % (0, 0))


if __name__ == '__main__':
    print(cmdLineMain())